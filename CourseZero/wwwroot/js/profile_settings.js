function showPWcol() {
    var ele = document.getElementById('profile_change_PW_form');
    if (ele.style.display == 'none') {
        ele.style.display = 'block';
    }
    else {
        ele.style.display = 'none';
    }
};
$(document).ready(function () {
    var profile_password_block = document.getElementById("Profile_pwBlock");
    var profile_confirm_block = document.getElementById("Profile_checkBlock");
    var profile_old_block = document.getElementById("Profile_oldpwBlock");

    function form_check_new_old() {
        Form_Login_Valid_Password($("#profile_newPassword").val(), profile_password_block);
        var temp = $("#profile_newPassword").val(), temp2 = $("#profile_oldPassword").val();
        if (temp2 == "") {
            Form_Login_changeValidTxt(profile_old_block, 0, "Please fill in old password.");
            return false;
        }
        else Form_Login_changeValidTxt(profile_old_block, 1);

        if (temp.localeCompare(temp2) == 0) {
            Form_Login_changeValidTxt(profile_password_block, 0, "New password cannot be same as old one.");
            return false;
        }
        else return true;
    }

    function form_check_same() {
        var temp = $("#profile_newPassword").val(), temp2 = $("#profile_confirmPassword").val();
        if (temp.localeCompare(temp2) == 0) {
            Form_Login_changeValidTxt(profile_confirm_block, 1);
            return true;
        }
        else {
            Form_Login_changeValidTxt(profile_confirm_block, 0, "Password does not match");
            return false;
        }
    }

    $("#Profile_frame_content").load("logout.html");
    $("#profile_sm_btn").click(function () {
        var ele = document.getElementById('Profile_frame_content');
        if (ele.style.display == 'none') {
            document.getElementById("profile_sm_btn").innerHTML = "Close Session Manager";
            ele.style.display = 'block';
        }
        else {
            ele.style.display = 'none';
            document.getElementById("profile_sm_btn").innerHTML = "Open Session Manager";
        }
    });



    $("#profile_newPassword").on('keyup', function () {
        form_check_new_old();
    });

    $("#profile_confirmPassword").on('keyup', function () {
        form_check_same();
    });

    $("#profile_pw_btn").click(function () {

        if (!form_check_new_old() || !form_check_same())
            return;
        var msg_to_send = {
            "auth_token": g_auth_token,
            "old_password": $("#profile_oldPassword").val(),
            "new_password": $("#profile_newPassword").val()
        }
        postJSON("/api/ChangePassword", (msg_to_send), function (obj) {
            $("#profile_oldPassword").prop('disabled', false);
            $("#profile_newPassword").prop('disabled', false);
            $("#profile_confirmPassword").prop('disabled', false);
            $("#profile_pw_btn").prop("disabled", false);
            if (obj["status_code"] == 0) //success
            {
                $("#Profile_ChangePW_DisplayMsg").css("color", "green");
                $("#Profile_ChangePW_DisplayMsg").html(obj["display_message"]);

            }
            else if (obj["status_code"] > 0) //fail
            {
                $("#Profile_ChangePW_DisplayMsg").css("color", "red");
                $("#Profile_ChangePW_DisplayMsg").html(obj["display_message"]);
            }
        }, function () {
            $("#profile_oldPassword").prop('disabled', false);
            $("#profile_newPassword").prop('disabled', false);
            $("#profile_confirmPassword").prop('disabled', false);
            $("#profile_pw_btn").prop("disabled", false);
            $("#Profile_ChangePW_DisplayMsg").css("color", "red");
            $("#Profile_ChangePW_DisplayMsg").html("Server error");
        });
    });
});
