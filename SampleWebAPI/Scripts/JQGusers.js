// GET ALL
function GetAllUsers()
{
     $.ajax({
                    type: "GET",
                    url: "http://localhost:60520/api/usersapi/",
                    contentType: "json",
                    dataType: "json",
                    success: function (data) {
                               $.each(data, function (key, value) {
                              //stringify
                              var jsonData = JSON.stringify(value);
                              //Parse JSON
                             var objData = $.parseJSON(jsonData);
                             var id = objData.uid;
                             var fname = objData.firstName;
                             var lname = objData.lastName; $('<tr><td><a href="manage/?uid=' + id + '">' + id + '</a></td><td>'
                                 + fname + '</td><td>' 
                                 + lname +  '</td><td>'
                                 + '<a href="#" onclick="DeleteUser(' + id + ')"> D </a></td><td>'
                                 + '<a href="#" onclick="CopyUser('+id+')"> C </a></td><td>'
                                 +'</td></tr>').appendTo('#Users');});
                     },
                     error: function (xhr) {
                             alert(xhr.responseText);
                    }
         });
}

//GET
function GetUserById(uid)
{
    if (uid == 0)
        $('#uid').text(uid);
    else {
        $.ajax({
            type: "GET",
            url: "http://localhost:60520/api/usersapi/" + uid,
            contentType: "json",
            dataType: "json",
            success: function(data) {
                //stringify
                var jsonData = JSON.stringify(data);
                //Parse JSON
                var objData = $.parseJSON(jsonData);
                var uid = objData.uid;
                var fname = objData.firstName;
                var lname = objData.lastName;
                $('#uid').text(uid);
                $('#firstName').val(fname);
                $('#lastName').val(lname);
            },
            error: function(xhr) {
                alert(xhr.responseText);
            }
        });
    }
}

//ADD or CREATE
function ManageUser()
{
    var uid = $("#uid").text();
    var method = "PUT";
    if (uid == 0)
        method = "POST";
    var UserData = {
        "uid": uid,
        "firstName": $("#firstName").val(),
        "lastName": $("#lastName").val()
    };
    $.ajax({
        type: method,
        url: "http://localhost:60520/api/usersapi/",
        data: JSON.stringify(UserData),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        processData: true,
        success: function (data, status, jqXHR) {
            //stringify
            var jsonData = JSON.stringify(data);
            //Parse JSON
            var objData = $.parseJSON(jsonData);
            var uid = objData.uid;
            $('#uid').text(uid);
            if(method =="POST")
                $('#operation').text("User created successfully!");    
            else
                $('#operation').text("User saved successfully!");    
        },
        error: function (xhr) {
                alert(xhr.responseText);
            }
    });
}

//UPDATE
/*function UpdateUser()
{
                        var UserData = {
                            "uid": $("#id").val(),
                            "firstName": $("#firstName").val(),
                            "lastName": $("lastName").val()
                         };
                        $.ajax({
                                    type: "PUT",
                                    url: "http://localhost:60520/api/usersapi/",
                                    data: JSON.stringify(UserData),
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    processData: true,
                                    success: function (data, status, jqXHR) {
                                                     alert("success…" + data);
                                     },
                          error: function (xhr) {
                                     alert(xhr.responseText);
                           }
            });
}*/


//DELETE
function DeleteUser(uid)
{
                    $.ajax({
                                  type: "DELETE",
                                  url: "http://localhost:60520/api/usersapi/"+uid,
                                  contentType: "json",
                                  dataType: "json",
                                  success: function (data) {
                                  alert("successs…. ");
                      },
                      error: function (xhr) {
                                   alert(xhr.responseText);
                       }
           });
}

//COPY
function CopyUser(uid) {
    $.ajax({
        type: "POST",
        url: "http://localhost:60520/api/usersapi/Copy/" + uid,
        contentType: "json",
        dataType: "json",
        success: function (data) {
            goTo("/users/");
        },
        error: function (xhr) {
            alert(xhr.responseText);
        }
    });
}

function GetQueryStringParams(sParam) {
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');

    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) {
            return sParameterName[1];
        }
    }
}

function goTo(url) {
    window.location = "http://"+$(location).attr('host') + url;
}