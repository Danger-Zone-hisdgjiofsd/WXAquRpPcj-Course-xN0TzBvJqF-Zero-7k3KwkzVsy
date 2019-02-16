 function changeValid_color(input,check) {
	if (check == 1) {
		input.classList.add('text-success');
		input.classList.remove('text-muted');
	}
	else input.classList.remove('text-success');
}


function show_SignUp() 
{
	document.getElementById('Login_Form').style.display = 'none';
	document.getElementById('Signup_Form').style.display = 'block';
}

function show_ForgotPW() 
{
	document.getElementById('Login_Form').style.display = 'none';
	document.getElementById('ForgotPW_Form').style.display = 'block';
}