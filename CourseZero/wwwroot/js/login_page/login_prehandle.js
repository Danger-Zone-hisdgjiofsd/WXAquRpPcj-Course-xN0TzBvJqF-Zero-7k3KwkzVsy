       //signin part
        $("#Signin_btn").click(function () {
            var msg_to_send = {
                "using_email": false,
                "username": "",
                "email": "",
                "password": null,
                "recaptcha_hash": "a"

            }

            temp = $("#account").val();
            if (temp.indexOf("@") !== -1) {
                msg_to_send.using_email = true;
                msg_to_send.email = $("#account").val();
                msg_to_send.username = "11550932"
            }
            else {
                msg_to_send.username = $("#account").val();
                msg_to_send.email = "1155090000@link.cuhk.edu.hk"
            }
            msg_to_send.password = $("#inputPassword").val();
            $.postJSON("/api/Login", JSON.stringify(msg_to_send));

        });

        //register part
        var msg_to_send = {
            "username": null,
            "password": null,
            "email": null,
            "recaptcha_hash": "string"
        };

        var email_block = document.getElementById("emailHelpBlock");
        var username_block = document.getElementById("nameHelpBlock");
        var password_block = document.getElementById("passwordHelpBlock");
        var confirm_block = document.getElementById("confirmHelpBlock");

        var mail_valid = false, username_valid = false, password_valid = false, confirm_valid = false, ticked = false;

        function allFill_In(id) {
            var target = document.getElementById(id);
            if (mail_valid && username_valid && password_valid && confirm_valid&&ticked)
                target.disabled = false;
            else {
                target.disabled = true;
                
            };
        }

        function confirmPW() {
            var temp = $("#Register_PW").val(), temp2 = $("#Register_ConfirmPW").val();

            if (temp.localeCompare(temp2) == 0) {
                msg_to_send.password = temp;
                confirm_block.innerHTML = "Two passwords are  matched.";
                confirm_valid = true;

                changeValid_color(confirm_block, 1);
            }
            else {
                msg_to_send.password = temp;
                confirm_block.innerHTML = "Two passwords are not matched.";
                changeValid_color(confirm_block, 0);
                confirm_valid = false;
            }
            allFill_In('signupbtn');
        }

        function checkBoxTicked() {
            var checkBox = document.getElementById("checkTick");
            if (checkBox.checked == true) {
                ticked = true;
            } else {
                ticked = false;
            }
            allFill_In('signupbtn');
        }

        $("#Register_Mail").on('keyup', function () {
            var temp = $("#Register_Mail").val();
            var domain = temp.substring(10);
            msg_to_send.email = temp;
            
            if (temp.length == 27 && domain.localeCompare("@link.cuhk.edu.hk") == 0 && temp.indexOf(" ") == -1) {
                email_block.innerHTML = "You email is valid.";
                mail_valid = true;
                changeValid_color(email_block, 1);
            }
            else {
                email_block.innerHTML = "You must use @link.cuhk.edu.hk email to register.";
                changeValid_color(email_block, 0);
                mail_valid = false;
            }
            allFill_In('signupbtn');
        }
        )

        $("#Register_UserName").on('keyup', function () {
            var temp = $("#Register_UserName").val();
            msg_to_send.username = temp;
            if (temp.length >= 5 && temp.length <= 20 && temp.indexOf(" ") == -1) {
                username_block.innerHTML = "Username is filled in.";
                username_valid = true;
                changeValid_color(username_block, 1);
            }
            else {
                username_block.innerHTML = "Username must be between 5 and 20 characters.";
                changeValid_color(username_block, 0);
                username_valid = false;
            }
            allFill_In('signupbtn');
        })

        $("#Register_PW").on('keyup', function () {
            var temp = $("#Register_PW").val();
            
            if (temp.length >= 5 && temp.length <= 20 && temp.indexOf(" ") == -1) {
                password_block.innerHTML = "Password is filled in.";
                password_valid = true;
               
                changeValid_color(password_block, 1);
            }
            else {
                password_block.innerHTML = "Password must be between 5 and 20 characters.";
                changeValid_color(password_block, 0);
                password_valid = false;
            }
            confirmPW();
        })

        $("#Register_ConfirmPW").on('keyup', function () {
            confirmPW();
        })

        $("#ForgotPW_email").on('keyup', function () {
            console.log("pk");
            username_valid = true; password_valid = true; confirm_valid = true; ticked = true;
            if ($("#ForgotPW_email").val() != "")
                mail_valid = true;
            else mail_valid = false;
            
            allFill_In('ForgotPW_btn');
        })

        $("#account").on('keyup', function () {
 
            username_valid = true; confirm_valid = true; ticked = true;
            if ($("#account").val() != "")
                mail_valid = true;
            else mail_valid = false;

            allFill_In('Signin_btn');
        })

        $("#inputPassword").on('keyup', function () {

            username_valid = true; confirm_valid = true; ticked = true;
            if ($("#inputPassword").val() != "")
                password_valid = true;
            else password_valid = false;

            allFill_In('Signin_btn');
        })
        
        $("#signupbtn").click(function () {

            $.postJSON("/api/Register", JSON.stringify(msg_to_send));
        });

        $("#ForgotPW_btn").click(function () {
            var msg_to_send = {
                "email": $("#ForgotPW_email").val(),
            }
            $.postJSON("/api/ForgotPassword/Request_Change", JSON.stringify(msg_to_send));
        });
        