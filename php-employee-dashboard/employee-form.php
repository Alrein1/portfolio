<?php
require('save-employee.php');

?>

<!DOCTYPE html>
<html lang="en">
<head>
    <link rel="stylesheet" href="styles.css">
    <meta charset="UTF-8">
    <title>Dashboard</title>
</head>
<body id="employee-form-page">
<div class="main">
    <?php include_once 'menu.php';
    ?>
    <div id="error-block">
        <?php
        if (isset($_GET['message'])) {
            $error = $_GET['message'] === 'error';
            if ($error) {
                echo '<ul>';
                echo '<li>First name must be between 1 and 21 characters</li>';
                echo '<li>Last name must be between 2 and 22 characters</li>';
                echo '</ul>';
            }
        }
        ?>
    </div>
    <form method="post" action="save-employee.php">
        <div class="employees-form">
            <div>
                <label for="firstName">First name:</label>
                <input name="firstName" id="firstName" value="<?php
                if (isset($_GET['firstName'])) {
                    $fn = $_GET['firstName'];
                    echo $fn;
                } else {
                    echo '';
                }
                ?>">
            </div>
            <div>
                <label for="lastName">Last name:</label>
                <input name="lastName" id="lastName" value="<?php
                if (isset($_GET['lastName'])) {
                    $ln = $_GET['lastName'];
                    echo $ln;
                } else {
                    echo '';
                }
                ?>">
            </div>
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