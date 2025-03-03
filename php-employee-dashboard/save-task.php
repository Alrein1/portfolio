<?php
require_once './data/connection.php';

$desc = $_POST['description'] ?? null;
$est = $_POST['estimate'] ?? null;
$workerId = $_POST['employeeId'] ?? null;

function validateInput(): array
{
    global $desc;
    $errors = [];
    if ($desc === null || strlen($desc) < 5 || strlen($desc) > 40) {
        $errors[] = 'Description must be between 5 and 40 characters';
    }
    return $errors;
}

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $conn = getDbConnection();

    if (isset($_POST['deleteButton'])) {
        $removeId = $_POST['deleteButton'];
        $stmt = $conn->prepare("DELETE FROM tasks WHERE id = :id");
        $stmt->bindParam(':id', $removeId, PDO::PARAM_INT);
        $stmt->execute();
        header('Location: task-list.php?message=task_removed');
        exit();
    }

    $errors = validateInput();
    if (empty($errors)) {
        $stmt = $conn->prepare("INSERT INTO tasks (description, estimate, worker_ID) VALUES (:desc, :est, :workerId)");
        $stmt->bindParam(':desc', $desc, PDO::PARAM_STR);
        $stmt->bindParam(':est', $est, PDO::PARAM_INT);
        $stmt->bindParam(':workerId', $workerId, PDO::PARAM_INT);
        $stmt->execute();
        header("Location: task-list.php?message=task_added");
        exit();
    }
    header("Location: task-form.php?message=error&desc=" . $desc);
    exit();
}

