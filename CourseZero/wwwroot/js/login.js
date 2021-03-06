function Form_Login_changeValidTxt(input, valid, showtxt = "") 
{
	if (input == null)
		return;
	input.innerHTML = showtxt;
	if (valid == 1) 
	{
		input.classList.add('text-success');
		input.classList.remove('text-muted');
	}
	else input.classList.remove('text-success');
}
function Form_Login_show_Login() {
	document.getElementById('Form_Login_Login_Form').style.display = 'block';
	document.getElementById('Form_Login_Signup_Form').style.display = 'none';
    document.getElementById('Form_Login_ForgotPW_Form').style.display = 'none';
    document.getElementById('Form_Login_Reissue_Form').style.display = 'none';
}
function Form_Login_show_SignUp() {
	document.getElementById('Form_Login_Login_Form').style.display = 'none';
	document.getElementById('Form_Login_Signup_Form').style.display = 'block';
    document.getElementById('Form_Login_ForgotPW_Form').style.display = 'none';
    document.getElementById('Form_Login_Reissue_Form').style.display = 'none';
}
function Form_Login_show_ForgotPW() {
	document.getElementById('Form_Login_Login_Form').style.display = 'none';
	document.getElementById('Form_Login_Signup_Form').style.display = 'none';
    document.getElementById('Form_Login_ForgotPW_Form').style.display = 'block';
    document.getElementById('Form_Login_Reissue_Form').style.display = 'none';
}
function Form_Login_show_Reissue() {
    document.getElementById('Form_Login_Login_Form').style.display = 'none';
    document.getElementById('Form_Login_Signup_Form').style.display = 'none';
    document.getElementById('Form_Login_ForgotPW_Form').style.display = 'none';
    document.getElementById('Form_Login_Reissue_Form').style.display = 'block';

}
function Form_Login_Valid_Username(username, block)
{
	if (username.length < 5 || username.length > 20)
	{
		Form_Login_changeValidTxt(block, 0, "Username must has length between 5 and 20");
        return false;
    }
    var letterdigit_count = 0;
    for (var i = 0; i < username.length; i++)
    {
        if ((username[i] >= '0' && username[i] <= '9') || (username[i] >= 'a' && username[i] <= 'z') || (username[i] >= 'A' && username[i] <= 'Z'))
            letterdigit_count++;
        else if (username[i] != '-' && username[i] != '_')
        {
            Form_Login_changeValidTxt(block, 0, "Username should contain only letter, digit, underscore (_) and hyphen (-).");
            return false
        }
    }
    if (letterdigit_count < 3) {
        Form_Login_changeValidTxt(block, 0, "Username should contain at least 3 letters or digits.");
        return false
    }
	Form_Login_changeValidTxt(block, 1);
	return true;
}
function Form_Login_Valid_Email(email, block)
{
	var error_msg = "You must use @link.cuhk.edu.hk email to register.";
	if (email.length != 27)
	{
		Form_Login_changeValidTxt(block, 0, error_msg);
		return false;
	}
	var domain = email.substring(10);
	var sid = email.substring(0, 10);
	if (domain != "@link.cuhk.edu.hk")
	{
		Form_Login_changeValidTxt(block, 0, error_msg);
		return false;
	}
	for (var i = 0; i < sid.length; i ++)
	{
		if (sid[i] < '0' || sid[i] > '9')
		{
			Form_Login_changeValidTxt(block, 0, error_msg);
			return false;
		}
	}
	Form_Login_changeValidTxt(block, 1);
	return true;
}

function Form_Login_Valid_Password(pword, block)
{
	if (pword.length < 5 || pword.length > 20)
	{
		Form_Login_changeValidTxt(block, 0, "Password must has length between 5 and 20");
		return false;
	}
	var specials = "~!@#$%^&*_-+=`|\\(){}[]:;\"'<>,.?/";
	for (var i = 0; i < pword.length; i++)
	{
		if (!((pword[i] >= '0' && pword[i] <= '9') || (pword[i] >= 'a' && pword[i] <= 'z') || (pword[i] >= 'A' && pword[i] <= 'Z') || specials.includes(pword[i])))
		{
			Form_Login_changeValidTxt(block, 0 , "Password should contain only letter, digit, special characters ~!@#$%^&*_-+=`|\\(){}[]:;\"'<>,.?/");
			return false;
		}
	}
	Form_Login_changeValidTxt(block, 1);
	return true;
}
$(document).ready(function () {
       //signin part

  

    $("#Form_Login_Signin_btn").click(function () {
            var msg_to_send = {
                "using_email": false,
                "password": null,
                "recaptcha_hash": "a"
            } 
            temp = $("#Form_Login_account").val();
            if (temp.indexOf("@") !== -1) {
				if (!Form_Login_Valid_Email($("#Form_Login_account").val(), null))
				{
					$("#Form_Login_Login_DisplayMsg").css("color", "red");
					$("#Form_Login_Login_DisplayMsg").html("Invalid Email!");
					return;
				}
                msg_to_send.using_email = true;
                msg_to_send["email"] = $("#Form_Login_account").val();
            }
            else {
				if (!Form_Login_Valid_Username($("#Form_Login_account").val(), null))
				{
					$("#Form_Login_Login_DisplayMsg").css("color", "red");
					$("#Form_Login_Login_DisplayMsg").html("Invalid username!");
					return;
				}
                msg_to_send["username"] = $("#Form_Login_account").val();
            }
			if (!Form_Login_Valid_Password($("#Form_Login_inputPassword").val(), null))
			{
				$("#Form_Login_Login_DisplayMsg").css("color", "red");
				$("#Form_Login_Login_DisplayMsg").html("Invalid password!");
				return;
			}
            msg_to_send.password = $("#Form_Login_inputPassword").val();
			$("#Form_Login_account").prop('disabled', true);
			$("#Form_Login_inputPassword").prop('disabled', true);
			$("#Form_Login_Remember_PW").prop("disabled", true);
			$("#Form_Login_Signin_btn").prop("disabled", true);
            postJSON("/api/Login", (msg_to_send), function(obj){
				$("#Form_Login_account").prop('disabled', false);
				$("#Form_Login_inputPassword").prop('disabled', false);
				$("#Form_Login_Remember_PW").prop("disabled", false);
				$("#Form_Login_Signin_btn").prop("disabled", false);
				if (obj["status_code"] == 0) //success 
				{
                    g_auth_token = obj["auth_token"];
                    g_username = obj["username"];
                   
                    g_login = true;
                    sessionStorage.saved_auth_token = obj["auth_token"];
                    if ($("#Form_Login_Remember_PW").is(":checked")) {
                        localStorage.saved_auth_token = obj["auth_token"];
                       
                    }

                    $("#Layout_loginbtn").hide();
                    $("#Layout_avatarbtn").show();
                    Paging_loadMain();
               
                    document.dispatchEvent(search_available);
				}
				else if (obj["status_code"] == 1) //fail
				{
                    $("#Form_Login_Login_DisplayMsg").css("color", "red");
                 
                       
                    $("#Form_Login_Login_DisplayMsg").html(obj["display_message"]);
                    if (obj["display_message"] == "Please verify your email first") {
                        var outmsg = document.getElementById("Form_Login_Login_DisplayMsg");
                        var word = "<a href=\"javascript:Form_Login_show_Reissue();\" style=\"color: blue;\">, need reissue?</a> ";
                        outmsg.innerHTML += word;
                        
                    }
				}
            }, function () {
                $("#Form_Login_account").prop('disabled', false);
                $("#Form_Login_inputPassword").prop('disabled', false);
                $("#Form_Login_Remember_PW").prop("disabled", false);
                $("#Form_Login_Signin_btn").prop("disabled", false);
                $("#Form_Login_Login_DisplayMsg").css("color", "red");
                $("#Form_Login_Login_DisplayMsg").html("Server error");
            });
        });

        //register part
        var email_block = document.getElementById("Form_Login_emailHelpBlock");
        var username_block = document.getElementById("Form_Login_nameHelpBlock");
        var password_block = document.getElementById("Form_Login_passwordHelpBlock");
        var confirm_block = document.getElementById("Form_Login_confirmHelpBlock");

        $("#Form_Login_Register_Mail").on('keyup', function () {
				Form_Login_Valid_Email($("#Form_Login_Register_Mail").val(), email_block);
			}
        )
        $("#Form_Login_Register_UserName").on('keyup', function () {
			Form_Login_Valid_Username($("#Form_Login_Register_UserName").val(), username_block)
        })

        $("#Form_Login_Register_PW").on('keyup', function () {
            Form_Login_Valid_Password($("#Form_Login_Register_PW").val(), password_block)
            Form_Login_changeValidTxt(confirm_block, 1);
        })

        $("#Register_ConfirmPW").on('keyup', function () {
            var temp = $("#Form_Login_Register_PW").val(), temp2 = $("#Register_ConfirmPW").val();
            if (temp.localeCompare(temp2) == 0)
                Form_Login_changeValidTxt(confirm_block, 1);
            else
				Form_Login_changeValidTxt(confirm_block, 0, "Password does not match");
        })


		$("#Form_Login_signupbtn").click(function () {
			var temp = $("#Form_Login_Register_PW").val(), temp2 = $("#Register_ConfirmPW").val();
            if (temp.localeCompare(temp2) == 0)
                Form_Login_changeValidTxt(confirm_block, 1);
            else
			{
				Form_Login_changeValidTxt(confirm_block, 0, "Password does not match");
				return;
			}
            if (!Form_Login_Valid_Email($("#Form_Login_Register_Mail").val(), email_block) || !Form_Login_Valid_Username($("#Form_Login_Register_UserName").val(), username_block) || !Form_Login_Valid_Password($("#Form_Login_Register_PW").val(), password_block) ) {
                //Form_Login_changeValidTxt(confirm_block, 0, "APassword does not match");
                return;
            }
                
			var msg_to_send =
			{
				"username": $("#Form_Login_Register_UserName").val(),
				"password": $("#Form_Login_Register_PW").val(),
				"email": $("#Form_Login_Register_Mail").val(),
				"recaptcha_hash": "string"
			}
			$("#Form_Login_Register_Mail").prop('disabled', true);
			$("#Form_Login_Register_UserName").prop('disabled', true);
			$("#Form_Login_Register_PW").prop('disabled', true);
			$("#Register_ConfirmPW").prop('disabled', true);
			$("#Form_Login_signupbtn").prop('disabled', true);
			postJSON("/api/Register", (msg_to_send), function(obj){
				$("#Form_Login_Register_Mail").prop('disabled', false);
				$("#Form_Login_Register_UserName").prop('disabled', false);
				$("#Form_Login_Register_PW").prop('disabled', false);
				$("#Register_ConfirmPW").prop('disabled', false);
				$("#Form_Login_signupbtn").prop('disabled', false);
				if (obj["status_code"] == 0) //success 
				{
					$("#Form_Login_Signup_DisplayMsg").css("color", "green");
				}
				else if (obj["status_code"] == 1) //fail
				{
					$("#Form_Login_Signup_DisplayMsg").css("color", "red");
				}
				$("#Form_Login_Signup_DisplayMsg").html(obj["display_message"]);
            }, function () {
                $("#Form_Login_Register_Mail").prop('disabled', false);
                $("#Form_Login_Register_UserName").prop('disabled', false);
                $("#Form_Login_Register_PW").prop('disabled', false);
                $("#Register_ConfirmPW").prop('disabled', false);
                $("#Form_Login_signupbtn").prop('disabled', false);
                $("#Form_Login_Signup_DisplayMsg").css("color", "red");
                $("#Form_Login_Signup_DisplayMsg").html("Server error");
            });
        });

        $("#Form_Login_ForgotPW_btn").click(function () {
			if (!Form_Login_Valid_Email($("#Form_Login_ForgotPW_email").val()))
			{
				$("#Form_Login_ForgotPW_DisplayMsg").css("color", "red");
				$("#Form_Login_ForgotPW_DisplayMsg").html("Invalid Email");
				return;
			}
            var msg_to_send = {
                "email": $("#Form_Login_ForgotPW_email").val(),
            }
			$("#Form_Login_ForgotPW_btn").prop('disabled', true);
			$("#Form_Login_ForgotPW_email").prop('disabled', true);
            postJSON("/api/ForgotPassword/Request_Change", (msg_to_send), function(obj)
			{
				$("#Form_Login_ForgotPW_btn").prop('disabled', false);
				$("#Form_Login_ForgotPW_email").prop('disabled', false);
				if (obj["status_code"] == 0) //success 
				{
					$("#Form_Login_ForgotPW_DisplayMsg").css("color", "green");
				}
				else if (obj["status_code"] == 1) //fail
				{
					$("#Form_Login_ForgotPW_DisplayMsg").css("color", "red");
				}
				$("#Form_Login_ForgotPW_DisplayMsg").html(obj["display_message"]);
            }, function () {
                $("#Form_Login_ForgotPW_btn").prop('disabled', false);
                $("#Form_Login_ForgotPW_email").prop('disabled', false);
                $("#Form_Login_ForgotPW_DisplayMsg").css("color", "red");
                $("#Form_Login_ForgotPW_DisplayMsg").html("Server error");
               });

    });

    $("#Form_Login_Reissus_btn").click(function () {
        if (!Form_Login_Valid_Email($("#Form_Login_Reissue_email").val())) {
            $("#Form_Login_Reissue_DisplayMsg").css("color", "red");
            $("#Form_Login_Reissue_DisplayMsg").html("Invalid Email");
            return;
        }

        var msg_to_send = {
            "email": $("#Form_Login_Reissue_email").val(),
        }
        $("#Form_Login_Reissue_btn").prop('disabled', true);
        $("#Form_Login_Reissue_email").prop('disabled', true);
        postJSON("/api/Register/Reissue_Email", (msg_to_send), function (obj) {
            $("#Form_Login_Reissue_btn").prop('disabled', false);
            $("#Form_Login_Reissue_email").prop('disabled', false);
            if (obj["status_code"] == 0) //success 
            {
                $("#Form_Login_Reissue_DisplayMsg").css("color", "green");
            }
            else if (obj["status_code"] == 1) //fail
            {
                $("#Form_Login_Reissue_DisplayMsg").css("color", "red");
            }
            $("#Form_Login_Reissue_DisplayMsg").html(obj["display_message"]);
        }, function () {
            $("#Form_Login_Reissue_btn").prop('disabled', false);
            $("#Form_Login_Reissue_email").prop('disabled', false);
            $("#Form_Login_Reissue_DisplayMsg").css("color", "red");
            $("#Form_Login_Reissue_DisplayMsg").html("Server error");
            });

    });
});
