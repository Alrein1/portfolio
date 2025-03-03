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
<body id="task-list-page">
<div class="main">
    <?php include_once 'menu.php'?>
    <div class="tasks-holder">
        <?php
        echo '<div id="message-block">';
        $message = $_GET['message'] ?? null;
        if(isset($message)) {
            if ($message === 'task_added') {
                echo '<h1>Task added</h1>';
            } else if ($message === 'task_removed') {
                echo '<h1>Task removed</h1>';
            }
        }
        echo '</div>';

        $stmt = $conn->prepare('SELECT * FROM tasks');
        $stmt->execute();

        foreach ($stmt as $row) {
            $desc = $row['description'];
            $est = $row['estimate'];
            $id = $row['id'];
            $workerId = $row['worker_ID'];
            echo '<div class="employee-card">';
            echo '<div data-task-id='. $id .'>';
            echo $desc;
            echo '</div>';
            echo '<p>Estimate: ' . $est . '</p>';
            $stmt2 = $conn->prepare('SELECT * FROM employees WHERE ID = :workerId');
            $stmt2->bindParam(':workerId', $workerId, PDO::PARAM_INT);
            $stmt2->execute();
            foreach ($stmt2 as $rowEmployee) {
                $fn = $rowEmployee['first_name'];
                $ln = $rowEmployee['last_name'];
                echo '<p>Assigned worker: ' . $fn . ' ' . $ln . '</p>';
                echo '<a id="task-edit-link-' .$id . '" href="task-form.php?edit=' .$id .'&worker=' . $workerId .'">Edit</a>';
            }
            echo '</div>';
            if (isset($_GET['remove'])) {
                header('Location: remove_task.php?remove=' . $_GET['remove']);
            }
        }
        ?>
    </div>
</div>
</body>
</html>