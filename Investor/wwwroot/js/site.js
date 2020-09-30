//Form input events
$(".formInput > input").focus(function () {
    $(this).parent().find("label").addClass("minid");
});

$(".formInput > input").focusout(function () {
    if ($(this).val() == "") {
    $(this).parent().find("label").removeClass("minid");
    }
});

//Login validation
function failInput($input) {
    $input.removeClass("is-valid");
    $input.addClass("is-invalid");
    return false;
}

function passInput($input) {
    $input.removeClass("is-invalid");
    $input.addClass("is-valid");
    return true;
}

function validateText($input) {
    if ($input.val() == "" || $input.val() == null) {
        return failInput($input);
    } else {
        return passInput($input);
    }
}

function validateEmail($input) {
    var emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i;

    if (!emailRegex.test($input.val())) {
        return failInput($input);
    }  else {
        return passInput($input);
    }
}

function validatePasswords($input1, $input2) {
    if ($input1.val() == "" || $input1.val() == null) {
        failInput($input1);
        return failInput($input2);
    } else {
        var passwordRegex = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$/gm;

        if (!passwordRegex.test($input1.val())) {
            failInput($input1);
            return failInput($input2);
        } else {
            if ($input1.val() == $input2.val()) {
                passInput($input1);
                return passInput($input2);
            } else {
                passInput($input1);
                return failInput($input2);
            }
        }
    }
}

function ajax(url, data, successFunc) {
    $.ajax({
        type: "POST",
        url: url,
        data: data,
        success: successFunc,
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }
    });
}

$("#registerBtn").click(function () {
    var emailV = validateEmail($("#registerEmail"));
    var usernameV = validateText($("#registerUsername"));
    var pwsV = validatePasswords($("#registerPassword"), $("#registerConfirmPassword"));

    if (emailV && usernameV && pwsV) {
        var account = new Object();
        account.Id = 0;
        account.Email = $("#registerEmail").val();
        account.Username = $("#registerUsername").val();
        account.Password = $("#registerPassword").val();
        account.Company = $("#registerCompany").prop("checked");

        ajax("Login/RegisterValidate", { newAccount: account }, function (response) {
            if (response == 0) { //VALID
                alert("Registration complete!");
                clearRegisterForm();
                //Redirect to a registration success page
            } else if (response == 1) { //INVALID_USERNAME
                alert("Username is taken");
            } else if (response == 2) { //INVALID_EMAIL
                alert("Email is taken");
            } else {
                alert("Incorrect code: " + response);
            }
        });
    }
});

function clearRegisterForm() {
    
}

$("#loginBtn").click(function () {
    //Check if login creds work or not with the db
    var usernameEmail = $("#loginUsernameEmail").val();

    ajax("Login/LoginValidate", { usernameEmail: usernameEmail, password: $("#loginPassword").val() }, function (response) {
        if (response == 0) { //VALID
            passInput($("#loginUsernameEmail"));
            passInput($("#loginPassword"));
            ajax("Login/LoginSuccess", { usernameEmail: usernameEmail }, function (response) {
                window.location.href = "/Home/Index";
            });
        } else if (response == 1 || response == 2) { //INVALID_USERNAME/INVALID_EMAIL
            failInput($("#loginUsernameEmail"));
        } else if (response == 3) { //INVALID_PASSWORD
            failInput($("#loginPassword"));
        } else {
            alert("Incorrect code: " + response);
        }
    });
});