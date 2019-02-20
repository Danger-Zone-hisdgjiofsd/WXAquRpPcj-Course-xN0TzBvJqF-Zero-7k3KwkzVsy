function Logout() {
    var msg_to_send = {
        "auth_token": null
    }
    msg_to_send.auth_token = g_auth_token;
    g_login = false;
    postJSON("/api/Session/Logout", msg_to_send);
    if (localStorage.getItem("saved_auth_token")!=null)
        localStorage.removeItem("saved_auth_token");
}

function Logout_Spec(token) {

    var msg_to_send = {
        "auth_token": null,
        "token_to_be_removed": null
    }

    msg_to_send.auth_token = g_auth_token;
    msg_to_send.token_to_be_removed = token;

    postJSON("/api/Session/Logout_Specific_Session", msg_to_send);
   
}

function adddiv(id) {

    var element = document.createElement('div');
    element.className = "pastupload row pt-4 pb-4";
    element.id = "Logout_Session_frame_" + id;
    element.setAttribute("onclick", "pickLogout(this);");
    var word = document.createElement('div');
    word.className = "col-8";

    var LAT = document.createElement('p');
    LAT.id = "Logout_Session_LAT_" + id;
    LAT.innerHTML = "Last Access Time:";
    var LAD = document.createElement('p');
    LAD.id = "Logout_Session_LAD_" + id;
    LAD.innerHTML = "Access Device:";
    var LAB = document.createElement('p');
    LAB.id = "Logout_Session_LAB_" + id;
    LAB.innerHTML = "Access Browser:";
    word.appendChild(LAT);
    word.appendChild(LAD);
    word.appendChild(LAB);

    element.appendChild(word);

    document.getElementsByClassName("container-fluid no-padding")[0].appendChild(element);

}

var sessions;
function getAllSession() {
    var msg_to_send = {
        "auth_token": null
    }
    console.log(g_auth_token);
    
    msg_to_send.auth_token = g_auth_token;
    console.log(msg_to_send);
    postJSON("/api/Session/Get_All_Sessions", msg_to_send, function (obj) {
        sessions = obj["sessions"];
        if (obj["status_code"] == 0) //success
        {
            
            var text;
            for (var i = 0; i < sessions.length; i++) {
                if (sessions[i].token == msg_to_send.auth_token) {
                    text = document.getElementById("Logout_Session_Current_device");
                    text.innerHTML = "Current Device: " + sessions[i].last_access_Device;
                    text = document.getElementById("Logout_Session_Current_browser");
                    text.innerHTML = "Access Browser: " + sessions[i].last_access_Browser;
                }

                else {
                    adddiv(i);
                    text = document.getElementById("Logout_Session_LAT_" + i);
                    text.innerHTML = "Last Access Time: " + sessions[i].last_access_Time.substring(0, 10) + " " + sessions[i].last_access_Time.substring(11, 19);
                    text = document.getElementById("Logout_Session_LAD_" + i);
                    text.innerHTML = "Last Access Device: " + sessions[i].last_access_Device;
                    text = document.getElementById("Logout_Session_LAB_" + i);
                    text.innerHTML = "Last Access Browser: " + sessions[i].last_access_Browser;
                }
            }
        }
    });
}

function pickLogout(obj) {
    if (confirm("Do you want to logout this session? ")) {
        var id = obj.getAttribute('id');
        var logout_select = id.substring(21);
        Logout_Spec(sessions[logout_select].token);
        obj.remove();
        alert("Done");
    }
}

function SignOut_Clicked()  {
    if (g_login) {
        if (confirm('Logout?')) {
            Logout();
            Paging_loadMain();
            $("#Layout_avatarbtn").hide();
            $("#Layout_loginbtn").show();
        }
    }
}