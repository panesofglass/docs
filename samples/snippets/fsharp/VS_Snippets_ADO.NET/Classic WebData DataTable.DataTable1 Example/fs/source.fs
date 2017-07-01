// If running in F# Interactive, reference System.Windows.Forms
#if INTERACTIVE
#r "System.Windows.Forms"
#endif

open System
open System.Data
open System.Windows.Forms

// <Snippet1>
let makeDataTableAndDisplay (dataGrid: DataGridView) =
    // Create new DataTable.
    let table = new DataTable("table")

    // Create new DataColumn, set DataType, 
    // ColumnName and add to DataTable.    
    let idColumn =
        new DataColumn(
            DataType = typeof<int>,
            ColumnName = "id")
    table.Columns.Add(idColumn)

    // Create second column.
    let itemColumn =
        new DataColumn(
            DataType = typeof<string>,
            ColumnName = "item")
    table.Columns.Add(itemColumn)

    // Create new DataRow objects and add to DataTable.    
    for i in 0..9 do
        let row = table.NewRow()
        row.["id"] <- i
        row.["item"] <- sprintf "item %i" i
        table.Rows.Add(row)

    // Set to DataGrid.DataSource property to the table.
    dataGrid.DataSource <- table
// </Snippet1>

let makeMainForm() =
    let dataGrid = new DataGridView(AutoGenerateColumns = true, Dock = DockStyle.Fill)
    makeDataTableAndDisplay dataGrid

    let mainForm = new Form(Width = 450, Height = 400, Text = "DataTable.DataTable1")
    mainForm.Controls.Add(dataGrid)
    mainForm

#if INTERACTIVE
// When running in F# Interactive, use Form.Show().
do
    let mainForm = makeMainForm()
    mainForm.Show()
#else
// Otherwise, specify the main function as the startup entry point.
[<EntryPoint; STAThread>]
let main args =
    let mainForm = makeMainForm()
    Application.Run(mainForm)
    0
#endif