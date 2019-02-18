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
		$("#Layout_frame_content").prop('disabled', true);
	}
	else
	{
		$("#Layout_spinner_layer").hide();
		$("#Layout_frame_content").prop('disabled', false);
	}
}

function Paging_Listitemload() { 
    $("li a").click(function (e) {
        e.preventDefault(); //To prevent the default anchor tag behaviour
        var url = this.href;
        Paging_load(url);
    });
}

$(document).ready(function () {
    Paging_loadMain();
    Paging_Listitemload();
	$("#Layout_loginbtn").click(Paging_loadLogin);
	$("#Layout_homebtn").click(Paging_loadMain);
	$("#navbar_logo").wrap('<a href="./newmainpage.html"></a>');
});
