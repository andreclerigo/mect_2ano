<?php
  if (isset($_POST['value'])) {
    $value = $_POST['value'];

    /* Conectando, escolhendo o banco de dados */
    $link = mysqli_connect("localhost", "root", "", "Reservatorio") or die("Nao pude conectar: " . mysqli_error($link));

    /* Fazendo a query SQL DE ATUALIZACAO DA BASE DE DADOS */
    $query = "UPDATE ledcontrol SET gpio2='$value' WHERE id=1";
    $result = mysqli_query($link, $query) or die("A query falhou: " . mysqli_error($link));

    /* Fechando a conexão */
    mysqli_close($link);

    // You can echo a response if needed
    echo "Value updated successfully to " . $value;
  }
?>
