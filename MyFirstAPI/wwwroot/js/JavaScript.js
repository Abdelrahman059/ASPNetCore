var connection = new signalR.HubConnectionBuilder().withUrl("/PostHub").withAutomaticReconnect([0, 1000, 5000, null])
    .configureLogging(signalR.LogLevel.Information).build();





connection.on("ReceiveMessage", function (Author, Title) {

    toastr.success(Title, 'Author' + " " + Author);
});

connection.start().then(function () {
    console.log("connected");
}).catch(function (err) {
    return console.error(err.toString());
    setTimeout(start, 5000);
});
