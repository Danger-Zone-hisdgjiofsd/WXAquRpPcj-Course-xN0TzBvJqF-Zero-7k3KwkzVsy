function Paging_loadMain()
{
	//To be done
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
        Paging_load("./advancedsearch.html");
        get_CourseCode_prefix();
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

$(document).ready(function () {
    Paging_loadMain();
    Paging_Listitemload();
	$("#Layout_loginbtn").click(Paging_loadLogin);
	$("#Layout_homebtn").click(Paging_loadMain);
    $("#navbar_logo").click(Paging_loadMain);
    Layoutbtn_display();
});
