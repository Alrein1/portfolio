<?php
require_once './data/connection.php';
$removeId = $_GET['remove'];
$conn = getDbConnection();

$stmt = $conn->prepare('DELETE FROM employees WHERE id = ' . $removeId);

header("Location: employee-list.php?message=employee_removed");
exit();