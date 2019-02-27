function Paging_loadMain()
{
	//To be done
    if (g_login) {
        document.getElementById("layout_Username").innerHTML = g_username;
    }
    Paging_load("./profile.html");
    
}
function Paging_loadLogin()
{
	if (!g_login)
		Paging_load("./testlogin.html");
}
function Paging_load(path)
{
	Paging_ShowSpinner(true);
	$("#Layout_frame_content").load(path, function() {
		Paging_ShowSpinner(false);
	});
}
function Paging_ShowSpinner(show)
{
	if (show)
	{
        $("#Layout_spinner_layer").show();
        $("#Layout_frame").stop(true, true).fadeOut('slow');
		$("#Layout_frame_content").prop('disabled', true);
	}
	else
	{
        $("#Layout_spinner_layer").hide();
        $("#Layout_frame").stop(true, true).fadeIn('slow');
		$("#Layout_frame_content").prop('disabled', false);
	}
}

function Paging_Listitemload_Notuse() { 
    $(".dropdown-menu li").click(function (event) {
        event.preventDefault();
        Paging_load(this.href);
    });
}

function Paging_Listitemload() { //For testing    
    $("#dropdownnameNicon").click(function (event) {
        event.preventDefault();
        Paging_load("./profile.html");
    });
    $("#dropdownprofile").click(function (event) {
        event.preventDefault();
        Paging_load("./profile.html");
    });
    $("#dropdownmyupload").click(function (event) {
        event.preventDefault();
        Paging_load("./myupload.html");
    });
    $("#dropdownhelp").click(function (event) {
        event.preventDefault();
        Paging_load("./help.html");
    });
    $("#dropdowncontact").click(function (event) {
        event.preventDefault();
        Paging_load("./contact.html");
    });
    $("#Layout_advancedsearch").click(function (event) {
        event.preventDefault();
        if (g_auth_token != "") {
            Paging_load("./advancedsearch.html");
        }
        else { 
            alert("Please login first!");
        }
    });
}

function Layoutbtn_display() {
    if (g_login == false) {
        $("#Layout_loginbtn").show();
        $("#Layout_avatarbtn").hide();
    }
    else { 
        $("#Layout_loginbtn").hide();
        $("#Layout_avatarbtn").show();
    }
}

function Auto_Login() {
    if (localStorage.getItem("saved_auth_token") != null || sessionStorage.saved_auth_token !=null) {

        var msg_to_send = {
            "auth_token": null 
        }
        if (sessionStorage.saved_auth_token != null)
            msg_to_send.auth_token = sessionStorage.getItem("saved_auth_token");
        else msg_to_send.auth_token = localStorage.getItem("saved_auth_token");

        postJSON("/api/General/GetUserInfo", (msg_to_send), function (obj) {
            if (obj["status_code"] == 0) //success 
            {
                g_auth_token = msg_to_send.auth_token;
                g_username = obj["username"];
                g_login = true;
                document.getElementById("layout_Username").innerHTML = g_username;
                $("#Layout_loginbtn").hide();
                $("#Layout_avatarbtn").show();
                document.dispatchEvent(search_available);
            }
        });

    }
}
$(document).ready(function () {
    Auto_Login();
    Paging_loadMain();
    Paging_Listitemload();
	$("#Layout_loginbtn").click(Paging_loadLogin);
	$("#Layout_homebtn").click(Paging_loadMain);
    $("#navbar_logo").click(Paging_loadMain);
    Layoutbtn_display();
});
