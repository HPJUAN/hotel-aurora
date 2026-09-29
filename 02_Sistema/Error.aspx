<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Error — Hotel Aurora</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />
    <style>
        :root { --brass: #c9a45c; }
        body  { background: #0f1524; color: #ece7db; font-family: 'Manrope', system-ui, sans-serif;
                display: grid; place-items: center; min-height: 100vh; margin: 0; }
        .box  { text-align: center; padding: 2rem; max-width: 480px; }
        .ico  { font-size: 4rem; color: var(--brass); opacity: .7; margin-bottom: 1rem; }
        h1    { font-family: Georgia, serif; font-size: 1.8rem; margin-bottom: .75rem; }
        p     { color: #8f9ab3; margin-bottom: 1.5rem; }
        .btn  { background: var(--brass); color: #1a1405; font-weight: 600;
                padding: .55rem 1.5rem; border-radius: 8px; text-decoration: none;
                display: inline-block; transition: background .18s; }
        .btn:hover { background: #e2c686; color: #1a1405; }
    </style>
</head>
<body>
    <div class="box">
        <div class="ico"><i class="bi bi-exclamation-circle"></i></div>
        <h1>Algo salió mal</h1>
        <p>
            Ocurrió un error inesperado en el sistema.<br />
            Si el problema persiste, contacta al administrador.
        </p>
        <a href="Default.aspx" class="btn">
            <i class="bi bi-house-door me-1"></i>Volver al inicio
        </a>
    </div>
</body>
</html>
