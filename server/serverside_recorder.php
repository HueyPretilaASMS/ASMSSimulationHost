<?php
$file = 'log.txt';
$data = $_POST['data'].";";

// using the FILE_APPEND flag to append the content to the end of the file
// and the LOCK_EX flag to prevent anyone else writing to the file at the same time
file_put_contents($file, $data, FILE_APPEND);
header('Location: /recorder/index.html'); // redirect back to the main site
?>