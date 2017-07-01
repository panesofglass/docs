// If running in F# Interactive, reference System.Windows.Forms
#if INTERACTIVE
#r "System.Windows.Forms"
#endif

open System
open System.Data
open System.Windows.Forms

// <Snippet1>
let makeParentTable() =
    // Create a new DataTable.
    let table = new DataTable("ParentTable")

    // Create new DataColumn, set DataType, 
    // ColumnName and add to DataTable.    
    let idColumn =
        new DataColumn(
            DataType = typeof<int>,
            ColumnName = "id",
            ReadOnly = true,
            Unique = true)
    // Add the Column to the DataColumnCollection.
    table.Columns.Add(idColumn)

    // Create second column.
    let parentColumn =
        new DataColumn(
            DataType = typeof<string>,
            ColumnName = "ParentItem",
            AutoIncrement = false,
            Caption = "ParentItem",
            ReadOnly = false,
            Unique = false)
    // Add the column to the table.
    table.Columns.Add(parentColumn)

    // Make the ID column the primary key column.
    table.PrimaryKey <- [| idColumn |]

    // Create three new DataRow objects and add 
    // them to the DataTable
    for i in 0..2 do
        let row = table.NewRow()
        row.["id"] <- i
        row.["ParentItem"] <- sprintf "ParentItem %i" i
        table.Rows.Add(row)
    
    table

let makeChildTable() =
    // Create a new DataTable.
    let table = new DataTable("ChildTable")

    // Create first column and add to the DataTable.
    let childIdColumn =
        new DataColumn(
            DataType = typeof<int>,
            ColumnName = "ChildID",
            AutoIncrement = true,
            Caption = "ID",
            ReadOnly = true,
            Unique = true)

    // Add the column to the DataColumnCollection.
    table.Columns.Add(childIdColumn)

    // Create second column.
    let childItemColumn =
        new DataColumn(
            DataType = typeof<string>,
            ColumnName = "ChildItem",
            AutoIncrement = false,
            Caption = "ChildItem",
            ReadOnly = false,
            Unique = false)
    table.Columns.Add(childItemColumn)

    // Create third column.
    let parentIdColumn =
        new DataColumn(
            DataType = typeof<int>,
            ColumnName = "ParentID",
            AutoIncrement = false,
            Caption = "ParentID",
            ReadOnly = false,
            Unique = false)
    table.Columns.Add(parentIdColumn)

    // Create three sets of DataRow objects, 
    // five rows each, and add to DataTable.
    for i in 0..4 do
        let row = table.NewRow()
        row.["childID"] <- i
        row.["ChildItem"] <- sprintf "Item %i" i
        row.["ParentID"] <- 0 
        table.Rows.Add(row)

    for i in 0..4 do
        let row = table.NewRow()
        row.["childID"] <- i + 5
        row.["ChildItem"] <- sprintf "Item %i" i
        row.["ParentID"] <- 1 
        table.Rows.Add(row)

    for i in 0..4 do
        let row = table.NewRow()
        row.["childID"] <- i + 10
        row.["ChildItem"] <- sprintf "Item %i" i
        row.["ParentID"] <- 2 
        table.Rows.Add(row)
    
    table

let makeDataSet() =
    // Run all of the functions. 
    let parentTable = makeParentTable()
    let childTable = makeChildTable()

    // Instantiate the DataSet variable.
    let dataSet = new DataSet()

    // Add the DataTables to the DataSet.
    dataSet.Tables.Add(parentTable)
    dataSet.Tables.Add(childTable)

    // DataRelation requires two DataColumn 
    // (parent and child) and a name.
    let parentColumn = parentTable.Columns.["id"]
    let childColumn = childTable.Columns.["ParentID"]
    let relation = DataRelation("parent2Child", parentColumn, childColumn)
    dataSet.Tables.["ChildTable"].ParentRelations.Add(relation)

    dataSet

let setDataGrid dataMember (dataGrid: DataGridView) =
    // Instruct the DataGrid to bind to the DataSet, with the 
    // ParentTable as the topmost DataTable.
    let dataSet = makeDataSet()
    let bindingSource = new BindingSource(dataSet, dataMember)
    dataGrid.DataSource <- bindingSource
// </Snippet1>

let makeMainForm() =
    let parentDataGrid = new DataGridView(AutoGenerateColumns = true, Dock = DockStyle.Top)
    setDataGrid "ParentTable" parentDataGrid
    let childDataGrid = new DataGridView(AutoGenerateColumns = true, Dock = DockStyle.Top)
    setDataGrid "ChildTable" childDataGrid
    let mainForm = new Form(Width = 450, Height = 400, Text = "DataSet Demo")
    mainForm.Controls.Add(childDataGrid)
    mainForm.Controls.Add(parentDataGrid)
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