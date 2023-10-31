<?php
    /* Conectando, escolhendo o banco de dados */
    $link = mysqli_connect("localhost", "root", "") or die("Nao pude conectar: " . mysqli_error());
    mysqli_select_db($link, "Reservatorio") or die("Nao pude selecionar o banco de dados");

    /* Fazendo a query SQL DE LEITURA DA BASE DE DADOS*/
    $query = "SELECT * FROM supervisaoreservatorio ORDER BY id DESC LIMIT 1";
    $result = mysqli_query($link, $query) or die("A query falhou: " . mysqli_error());
    $row = mysqli_fetch_assoc($result);

    /* Fazendo a query SQL DE LEITURA DA BASE DE DADOS*/
    $query_led = "SELECT * FROM ledcontrol WHERE id=1";
    $result_led = mysqli_query($link, $query_led) or die("A query falhou: " . mysqli_error());
    $linha_led = mysqli_fetch_array($result_led, MYSQLI_ASSOC);
    
    // /* Mudando o valor do gpio0Status */
    // if ($row['X0'] > 0) {
    //     echo '<script>document.getElementById("gpio0Status").innerHTML = "ON";</script>';
    //     echo '<script>document.getElementById("gpio0Status").style.color = "green";</script>';
    // } else {
    //     echo '<script>document.getElementById("gpio0Status").innerHTML = "OFF";</script>';
    //     echo '<script>document.getElementById("gpio0Status").style.color = "red";</script>';
    // }

    // /* Mudando o valor do gpio4Status */
    // if ($row['X1'] > 0) {
    //     echo '<script>document.getElementById("gpio4Status").innerHTML = "ON";</script>';
    //     echo '<script>document.getElementById("gpio4Status").style.color = "green";</script>';
    // } else {
    //     echo '<script>document.getElementById("gpio4Status").innerHTML = "OFF";</script>';
    //     echo '<script>document.getElementById("gpio4Status").style.color = "red";</script>';
    // }

    // /* Mudando o valor do gpio2Status */
    // if ($row['X2'] > 0) {
    //     echo '<script>document.getElementById("gpio2Status").innerHTML = "ON";</script>';
    //     echo '<script>document.getElementById("gpio2Status").style.color = "green";</script>';
    // } else {
    //     echo '<script>document.getElementById("gpio2Status").innerHTML = "OFF";</script>';
    //     echo '<script>document.getElementById("gpio2Status").style.color = "red";</script>';
    // }

    // Construct an array of data to be returned
    $data = array(
        'gpio0Status' => ($row['X0'] > 0) ? 'OFF' : 'ON',
        'gpio4Status' => ($row['X1'] > 0) ? 'OFF' : 'ON',
        'gpio2Status' => ($linha_led['gpio2'] > 0) ? 'OFF' : 'ON',
        'analog0Value' => $row['X3']
    );

    header('Content-Type: application/json');
    // Convert the array to JSON and echo it
    echo json_encode($data);

    /* Liberta o resultado */
    mysqli_free_result($result);
    /* Fechando a conexão */
    mysqli_close($link);
?>
