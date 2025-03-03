<?php
$removeId = $_GET['remove'];
$lines = file('data/tasks.txt', FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);
file_put_contents('data/tasks.txt', '');
$outputFile = fopen('data/tasks.txt', 'a');
$newId = 0;
foreach ($lines as $line) {
    $data = explode('###', $line);
    if (sizeof($data) === 3) {
        $desc = $data[0];
        $est = $data[1];
        $id = $data[2];
        $newId++;
        if ($id !== $removeId) {
            fwrite($outputFile, sprintf('%s###%s###%d', $desc, $est, $newId) . PHP_EOL);
        }
    }
}
fclose($outputFile);

header("Location: task-list.php?message=task_removed");
exit();