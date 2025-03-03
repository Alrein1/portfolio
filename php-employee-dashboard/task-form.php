<?php
require('save-task.php');
require_once './data/connection.php';

?>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <link rel="stylesheet" href="styles.css">
    <title>Add task</title>
</head>
<body id="task-form-page">
<div class="main">
    <?php include_once 'menu.php'?>
    <div id="error-block">
        <?php
        if (isset($_GET['message'])) {
            $error = $_GET['message'] === 'error';
            if ($error) {
                echo '<ul>';
                echo '<li>Task description must be 5 to 40 characters</li>';
                echo '</ul>';
            }
        }
        ?>
    </div>
    <form method="post" action="save-task.php">
        <div class="task-form">
            <div>
                <label for="description">Description:</label>
                <input name="description" id="description" value="<?php
                if (isset($_GET['desc'])) {
                    $desc = $_GET['desc'];
                    echo $desc;
                } else {
                    echo '';
                }
                ?>">
            </div>
            <div>
                <label><input type="radio" value="1" id="estimate" name="estimate">1</label>
                <label><input type="radio" value="2" id="estimate" name="estimate">2</label>
                <label><input type="radio" value="3" id="estimate" name="estimate">3</label>
                <label><input type="radio" value="4" id="estimate" name="estimate">4</label>
                <label><input type="radio" value="5" id="estimate" name="estimate">5</label>
            </div>
            <label for="employeeId">Select employee: </label>
            <select id="employeeId" name="employeeId">
                <?php
                $conn = getDbConnection();
                $stmt = $conn->prepare('SELECT * FROM employees');
                $stmt->execute();
                foreach ($stmt as $row) {
                    $fn = $row['first_name'];
                    $ln = $row['last_name'];
                    $id = $row['id'];
                    if (isset($_GET['worker'])) {
                        if ($_GET['worker'] == $id) {
                            echo '<option value="' . $id . '" selected="selected">' . $fn . ' ' . $ln . '</option>';
                            continue;
                        }
                    }
                    echo '<option value="' . $id . '">' . $fn . ' ' . $ln . '</option>';
                }
                ?>
            </select>
            <?php
            if (isset($_GET['edit'])) {
                $id = $_GET['edit'];
                echo '<button type="submit" id="deleteButton" name="deleteButton" value="' . $id . '">Delete</button>';
            }
            ?>

            <button name="submitButton" type="submit">Save</button>
        </div>
    </form>
</div>
</body>
</html>