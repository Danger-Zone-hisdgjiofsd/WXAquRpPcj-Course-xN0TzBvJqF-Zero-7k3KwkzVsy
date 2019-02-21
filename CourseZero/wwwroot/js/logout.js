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
    word.className = "col-6";
    var picframe = document.createElement('div');
    var pic = document.createElement('img');
    picframe.className = "col-3"
    pic.setAttribute("src", "./material/chrome.png");
    pic.setAttribute("style", "width:50px;height:50px;");
    pic.id = "Logout_Session_Pic_" + id;
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
    picframe.appendChild(pic);
    element.appendChild(word);
    element.appendChild(picframe);
    document.getElementsByClassName("container-fluid no-padding")[0].appendChild(element);

}

var sessions,session_left;
function checkShow() {
    if (session_left > 1)
        document.getElementById("Logout_Session_btn").style.display = "block";
    else document.getElementById("Logout_Session_btn").style.display = "none";
}
function getAllSession() {
    var msg_to_send = {
        "auth_token": null
    }
  
    msg_to_send.auth_token = g_auth_token;

    postJSON("/api/Session/Get_All_Sessions", msg_to_send, function (obj) {
        sessions = obj["sessions"];
        if (obj["status_code"] == 0) //success
        {
            session_left = sessions.length;
            console.log(sessions.length);
            console.log(session_left);
            checkShow();
            
            var text;
            var browser = ["chrome", "edge", "internet explorer", "firefox", "sarafi", "opera"];
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
                    text = document.getElementById("Logout_Session_Pic_" + i);
                    

              

                }
                for (var j = 0; j < browser.length; j++) {
                    if (sessions[i].last_access_Browser.toLowerCase().includes(browser[j])) {

                        var output = "./material/" + browser[j] + ".png";
                        text.setAttribute("src", output);
                    }
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
        session_left--;
        checkShow();
        alert("Done");
    }
}

function SignOut_Clicked()  {
    if (g_login) {
        if (confirm('Logout?')) {
            Logout();
            Paging_loadMain();
            $("#dropdown_avatar").hide();
            $("#Layout_loginbtn").show();

        }
    }
}

function logout_All_Session() {
    for (var i = 0; i < sessions.length; i++) {
        if (sessions[i].token != g_auth_token) {
            Logout_Spec(sessions[i].token);
            var remo = "#Logout_Session_frame_" + i;
            $(remo).remove();
        }
    }
}

