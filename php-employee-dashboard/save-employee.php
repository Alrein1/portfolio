<?php
require_once './data/connection.php';
$firstName = $_POST['firstName'] ?? null;
$lastName = $_POST['lastName'] ?? null;

function validateInput() : array
{
    global $firstName;
    global $lastName;
    $errors = [];
    if ($firstName === null || 1 > strlen($firstName) || strlen($firstName) > 22 ) {
        $errors[] = 'First name must be between 1 and 21 characters';
    }

    if ($lastName === null || 2 > strlen($lastName) || strlen($lastName) > 23 ) {
        $errors[] = 'Last name must be between 2 and 22 characters';
    }
    return $errors;
}

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $conn = getDbConnection();

    if (isset($_POST['deleteButton'])) {
        $removeId = $_POST['deleteButton'];
        $stmt = $conn->prepare("DELETE FROM employees WHERE id = :id");
        $stmt->bindParam(':id', $removeId, PDO::PARAM_INT);
        $stmt->execute();
        header('Location: employee-list.php?message=employee_removed');
        exit();
        }
    $errors = validateInput();

    if (empty($errors)) {
        $stmt = $conn->prepare("INSERT INTO employees (first_name, last_name) VALUES (:firstName, :lastName)");
        $stmt->bindParam(':firstName', $firstName, PDO::PARAM_STR);
        $stmt->bindParam(':lastName', $lastName, PDO::PARAM_STR);
        $stmt->execute();
        header("Location: employee-list.php?message=employee_added");
        exit();
    }
    header("Location: employee-form.php?message=error&firstName=" . $firstName . "&lastName=" . $lastName);
    exit();

}
?>