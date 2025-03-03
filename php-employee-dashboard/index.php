<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <link rel="stylesheet" href="hw/styles.css">
    <title>Dashboard</title>
</head>
<body id="dashboard-page">
<div class="main">
    <nav>
        <a href="." id="dashboard-link">Dashboard</a>
        <a href="./employee-list.php" id="employee-list-link">Employees</a>
        <a href="./employee-form.php" id="employee-form-link">Add employee</a>
        <a href="./task-list.php" id="task-list-link">Tasks</a>
        <a href="./task-form.php" id="task-form-link">Add task</a>
    </nav>

    <div class="dashboard">
        <div class="dashboard-employees">
            <h3 class="dashboard-title">Employees</h3>
            <hr>
        </div>
        <div class="dashboard-tasks">
            <h3 class="dashboard-title">Tasks</h3>
            <hr>
        </div>
    </div>
</div>
    <table width="100%">
        <colgroup>
            <col width="10%">
            <col width="10%">
            <col width="10%">
            <col width="10%">
            <col width="10%">
            <col width="10%">
            <col width="10%">
            <col width="10%">
            <col width="10%">
            <col width="10%">
        </colgroup>
        <tr>
            <td colspan="3"></td>
            <td colspan="4"><a href="index.html" id="dashboard-link">Dashboard</a> | <a href="hw/employee-list.php" id="employee-list-link">Employees</a> | <a href="hw/employee-form.php" id="employee-form-link">Add employee</a> | <a href="hw/task-list.php" id="task-list-link">Tasks</a> | <a href="hw/task-form.php" id="task-form-link">Add task</a></td>
            <td colspan="3"></td>
        </tr>
        <tr>
            <th colspan="3"></th>
            <th>Employees</th>
            <th colspan="3">Tasks</th>
        </tr>
        <tr>
            <td colspan="3"></td>
            <td>
                <table border="1">
                    <tr>
                        <th>Name</th>
                        <th>Position</th>
                    </tr>
                    <tr>
                        <td>Daisy smith</td>
                        <td>Manager</td>
                    </tr>
                    <tr>
                        <td>James Adams</td>
                        <td>Designer</td>
                    </tr>
                    <tr>
                        <td>Mary Brown</td>
                        <td>Developer</td>
                    </tr>
                </table>
            </td>
            <td colspan="3">
                <table border="1" width="100%">
                    <colgroup>
                        <col width="50%">
                        <col width="50%">
                    </colgroup>
                    <tr>
                        <td colspan="2">Task1</td>
                    </tr>
                    <tr>
                        <td>Priority 4</td>
                        <td>Open</td>
                    </tr>
                </table>
                <table border="1" width="100%">
                    <colgroup>
                        <col width="50%">
                        <col width="50%">
                    </colgroup>
                    <tr>
                        <td colspan="2">Task1</td>
                    </tr>
                    <tr>
                        <td>Priority 2</td>
                        <td>Pending</td>
                    </tr>
                </table>
                <table border="1" width="100%">
                    <colgroup>
                        <col width="50%">
                        <col width="50%">
                    </colgroup>
                    <tr>
                        <td colspan="2">Task1</td>
                    </tr>
                    <tr>
                        <td>Priority 3</td>
                        <td>Closed</td>
                    </tr>
                </table>
            </td>
            <td colspan="3"></td>
        </tr>
        <tr>
            <td colspan="10" height="100%"></td>
        </tr>
        <tr valign="bottom">
            <td colspan="3"></td>
            <td colspan="4"><hr>Footer</td>
            <td colspan="3"></td>
        </tr>
    </table>
</body>
</html>