<?php
require_once './data/connection.php';
$conn = getDbConnection();
?>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <link rel="stylesheet" href="styles.css">
    <title>Dashboard</title>
</head>
<body id="employee-list-page">
<div class="main">
        <?php
        include_once 'menu.php';
        echo '<div id="message-block">';
        $message = $_GET['message'] ?? null;
        if(isset($message)) {
            if ($message === 'employee_added') {
                echo '<h1>Employee added</h1>';
            } else if ($message === 'employee_removed') {
                echo '<h1>Employee removed</h1>';
            }
        }
        echo '</div>';
        $stmt = $conn->prepare("SELECT * FROM employees");
        $stmt->execute();

        foreach ($stmt as $row) {
            $firstName = $row['first_name'];
            $lastName = $row['last_name'];
            $id = $row['id'];
            echo '<div class="employee-card">';
            echo '<div data-employee-id=' . $id . '>';
            echo $firstName . ' ' . $lastName;
            echo '</div>';
            echo '<a id="employee-edit-link-' . $id . '" href="employee-form.php?edit=' . $id . '">Edit</a>';
            echo '</div>';
            if (isset($_GET['remove'])) {
                header('Location: remove_employee.php?remove=' . $_GET['remove']);
            }
        }
        ?>
</div>
</body>
</html>
