# Análisis técnico de NEXUS

## 1. Objetivo

El objetivo de este documento es comparar algunas opciones para mejorar el videojuego NEXUS y elegir las herramientas más adecuadas para desarrollar su nueva versión.

## 2. Alternativas evaluadas

### 2.1. Tipo de interfaz

**Alternativa 1: Mantener la consola**

- Ventajas: ya existe código desarrollado y es fácil continuar trabajando sobre él.
- Desventajas: el juego tendría una apariencia sencilla y pocas opciones visuales.

**Alternativa 2: Utilizar Windows Forms**

- Ventajas: permite crear ventanas, botones y una interfaz gráfica más atractiva.
- Desventajas: será necesario adaptar parte del código actual.

**Decisión:** se recomienda utilizar Windows Forms para mejorar la experiencia del jugador. Microsoft Learn confirma que permite crear aplicaciones de escritorio con controles gráficos y responder a las acciones del usuario.

### 2.2. Forma de guardar las partidas

**Alternativa 1: Archivos JSON**

- Ventajas: son sencillos de utilizar y NEXUS ya los utiliza para guardar algunos datos.
- Desventajas: será necesario organizar mejor la información para guardar el inventario y el progreso completo.

**Alternativa 2: SQL Server**

- Ventajas: permite organizar la información en tablas y consultar los datos guardados.
- Desventajas: requiere crear la base de datos y conectar el programa con ella.

**Decisión:** se recomienda SQL Server porque el proyecto necesita guardar y recuperar diferentes datos de las partidas. El archivo JSON podrá servir como referencia durante la transición.

### 2.3. Organización del código

**Alternativa 1: Mantener todo en una sola clase principal**

- Ventajas: es más sencillo al comenzar.
- Desventajas: cuando se agreguen nuevas funciones, será más difícil mantener y corregir el código.

**Alternativa 2: Separar las funciones en diferentes clases**

- Ventajas: permite organizar mejor el código y facilita encontrar errores.
- Desventajas: requiere organizar y adaptar algunas partes del programa.

**Decisión:** se recomienda separar la interfaz gráfica, las reglas del juego y el acceso a la base de datos.

## 3. Decisiones recomendadas

Para desarrollar la nueva versión de NEXUS se recomienda:

- Continuar utilizando C#.
- Crear la interfaz gráfica con Windows Forms.
- Utilizar SQL Server para guardar y recuperar las partidas.
- Reutilizar las clases que ya funcionan en la versión actual.
- Desarrollar las mejoras por etapas y probar cada una antes de continuar.

Estas decisiones permiten mejorar el juego sin tener que comenzar todo desde cero.

## 4. Fuentes técnicas consultadas

Se consultaron las siguientes fuentes oficiales para comprobar las características de las tecnologías propuestas:

- **Microsoft Learn — Windows Forms:** explica cómo crear aplicaciones de escritorio con ventanas y controles gráficos.  
  https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/

- **Microsoft Learn — SQL Server y ADO.NET:** explica cómo conectar aplicaciones .NET con SQL Server y consultar o modificar información.  
  https://learn.microsoft.com/en-us/sql/connect/ado-net/

## 5. Verificación de información obtenida con inteligencia artificial

La inteligencia artificial se utilizó como apoyo para organizar ideas, comparar alternativas y preparar la documentación técnica del proyecto.

Las recomendaciones se contrastaron con la documentación oficial de Microsoft para comprobar las características generales de Windows Forms y del acceso a SQL Server.

La información sobre el código actual se tomó como referencia del proyecto NEXUS desarrollado en H3. Las funciones nuevas descritas en los documentos son propuestas y todavía deben implementarse y probarse.

## 6. Conclusión

Después de comparar las alternativas, se recomienda desarrollar NEXUS con C#, Windows Forms y SQL Server.

Esta combinación permitirá mejorar la presentación del juego, organizar mejor el código y guardar más información sobre el progreso del jugador.

El desarrollo se realizará poco a poco, empezando por adaptar la misión existente y después incorporando las nuevas misiones.
