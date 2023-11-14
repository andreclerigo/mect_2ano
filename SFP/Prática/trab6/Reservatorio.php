<?php
  $HistNivel = [];
  $HistX1 = [];

  /* Ligacao a base de dados “Reservatorio” */
  $link = mysqli_connect("localhost", "root", "") or die("Nao pude conectar: " . mysqli_error());
  mysqli_select_db($link, "Reservatorio") or die("Nao pude selecionar o banco de dados");

  /* Ler tabela supervisaoreservatorio */
  $query = "SELECT * FROM supervisaoreservatorio";
  $result = mysqli_query($link, $query) or die("A query falhou: " . mysqli_error());

  while ($row = mysqli_fetch_array($result, MYSQLI_ASSOC)) {
    $HistNivel[] = (int)($row['X3'] / 1024 * 100);
    $HistX1[] = (int)$row['X1'];
  }

  /* Liberta o resultado */
  mysqli_free_result($result);
  /* Fechando a conexao */
  mysqli_close($link);

  echo json_encode(['HistNivel' => $HistNivel, 'HistX1' => $HistX1]); 
?>
