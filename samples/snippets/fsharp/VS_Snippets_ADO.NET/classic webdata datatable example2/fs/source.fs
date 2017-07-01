// <Snippet1>
open System
open System.Data

let createOrderTable() =
    let orderTable = new DataTable("Order")

    // Define one column.
    let colId = new DataColumn("OrderId", typeof<string>)
    orderTable.Columns.Add(colId)

    let colDate = new DataColumn("OrderDate", typeof<DateTime>)
    orderTable.Columns.Add(colDate)

    // Set the OrderId column as the primary key.
    orderTable.PrimaryKey <- [| colId |]

    orderTable

let createOrderDetailTable() =
    let orderDetailTable = new DataTable("OrderDetail")

    // Define all the columns once.
    let cols =
        [|
            new DataColumn("OrderDetailId",typeof<int>)
            new DataColumn("OrderId",typeof<string>)
            new DataColumn("Product",typeof<string>)
            new DataColumn("UnitPrice",typeof<decimal>)
            new DataColumn("OrderQty",typeof<int>)
            new DataColumn("LineTotal",typeof<decimal>,"UnitPrice*OrderQty")
        |]

    orderDetailTable.Columns.AddRange(cols)
    orderDetailTable.PrimaryKey <- [| orderDetailTable.Columns.["OrderDetailId"] |]
    orderDetailTable

let insertOrders(orderTable: DataTable) =
    // Add one row once.
    let row1 = orderTable.NewRow()
    row1.["OrderId"] <- "O0001"
    row1.["OrderDate"] <- DateTime(2013, 3, 1)
    orderTable.Rows.Add(row1)

    let row2 = orderTable.NewRow()
    row2.["OrderId"] <- "O0002"
    row2.["OrderDate"] <- DateTime(2013, 3, 12)
    orderTable.Rows.Add(row2)

    let row3 = orderTable.NewRow()
    row3.["OrderId"] <- "O0003"
    row3.["OrderDate"] <- DateTime(2013, 3, 20)
    orderTable.Rows.Add(row3)

let insertOrderDetails(orderDetailTable: DataTable) =
    // Use an Object array to insert all the rows .
    // Values in the array are matched sequentially to the columns, based on the order in which they appear in the table.
    let rows = 
        [|
            [|box 1;box "O0001";box "Mountain Bike";box 1419.5;box 36|]
            [|box 2;box "O0001";box "Road Bike";box 1233.6;box 16|]
            [|box 3;box "O0001";box "Touring Bike";box 1653.3;box 32|]
            [|box 4;box "O0002";box "Mountain Bike";box 1419.5;box 24|]
            [|box 5;box "O0002";box "Road Bike";box 1233.6;box 12|]
            [|box 6;box "O0003";box "Mountain Bike";box 1419.5;box 48|]
            [|box 7;box "O0003";box "Touring Bike";box 1653.3;box 8|]
        |]

    for row in rows do
        orderDetailTable.Rows.Add(row) |> ignore

let showTable(table: DataTable) =
    for col in table.Columns do
        Console.Write("{0,-14}", col.ColumnName)
    Console.WriteLine()

    for row in table.Rows do
        for col in table.Columns do
            if col.DataType.Equals(typeof<DateTime>) then
                Console.Write("{0,-14:d}", row.[col])
            elif col.DataType.Equals(typeof<decimal>) then
                Console.Write("{0,-14:C}", row.[col])
            else
                Console.Write("{0,-14}", row.[col]);           
        Console.WriteLine()
    Console.WriteLine()

let run() =
    // Create two tables and add them into the DataSet
    let orderTable = createOrderTable()
    let orderDetailTable = createOrderDetailTable()
    let salesSet = new DataSet()
    salesSet.Tables.Add(orderTable)
    salesSet.Tables.Add(orderDetailTable)

    // Set the relations between the tables and create the related constraint.
    salesSet.Relations.Add("OrderOrderDetail", orderTable.Columns.["OrderId"], orderDetailTable.Columns.["OrderId"], true) |> ignore

    Console.WriteLine("After creating the foreign key constriant, you will see the following error if inserting order detail with the wrong OrderId: ")
    try
        let errorRow = orderDetailTable.NewRow()
        errorRow.[0] <- 1
        errorRow.[1] <- "O0007"
        orderDetailTable.Rows.Add(errorRow)
    with e ->
        Console.WriteLine(e.Message)
    Console.WriteLine()

    // Insert the rows into the table
    insertOrders(orderTable)
    insertOrderDetails(orderDetailTable)

    Console.WriteLine("The initial Order table.")
    showTable(orderTable)

    Console.WriteLine("The OrderDetail table.")
    showTable(orderDetailTable)

    // Use the Aggregate-Sum on the child table column to get the result.
    let colSub = new DataColumn("SubTotal", typeof<decimal>, "Sum(Child.LineTotal)")
    orderTable.Columns.Add(colSub)

    // Compute the tax by referencing the SubTotal expression column.
    let colTax = new DataColumn("Tax", typeof<decimal>, "SubTotal*0.1")
    orderTable.Columns.Add(colTax)

    // If the OrderId is 'Total', compute the due on all orders; or compute the due on this order.
    let colTotal = new DataColumn("TotalDue", typeof<decimal>, "IIF(OrderId='Total',Sum(SubTotal)+Sum(Tax),SubTotal+Tax)")
    orderTable.Columns.Add(colTotal)

    let row = orderTable.NewRow()
    row.["OrderId"] <- "Total"
    orderTable.Rows.Add(row)

    Console.WriteLine("The Order table with the expression columns.")
    showTable(orderTable)
// </Snippet1>

#if INTERACTIVE
// If running in F# Interactive, simply call run()
do run()
#else
// Otherwise, specify the program entry point.
[<EntryPoint; STAThread>]
let main args =
    run()
    Console.WriteLine("Press any key to exit.....")
    Console.ReadKey() |> ignore
    0
#endif