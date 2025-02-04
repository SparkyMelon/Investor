﻿//Form input events
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

//Dragging logic
$(".profileSubContainerItem").mousedown(function (e) {
    e.preventDefault();
});
$(".profileSubContainerItem").mouseup(function (e) {
    e.preventDefault();
});
$(".profileSubContainerItem").mousemove(function (e) {
    e.preventDefault();
});

var mouseDown = false;
var mouseX = 0;
$(".profileSubContainer").mousedown(function (e) {
    mouseDown = true;
    mouseX = e.pageX;
});

$(".profileSubContainer").mouseup(function (e) {
    mouseDown = false;
});

$(".profileSubContainer").mousemove(function (e) {
    if (mouseDown) {
        if (mouseX != e.pageX) {
            scrollProfileSubContainer($(this), e.pageX - mouseX);
            mouseX = e.pageX;
        }
    }
});

$(".profileSubContainer").mouseleave(function () {
    mouseDown = false;
});

function scrollProfileSubContainer($container, moveAmount) {
    var minX = -580; //-910 for 6 panels
    var maxX = 0;

    if ($container.css("left") == null) {
        $container.css("left", "0px");
    }
    var curLeft = $container.css("left");
    curLeft = Number(curLeft.substring(0, curLeft.length - 2));
    if (curLeft == NaN) {
        alert("Oops..");
    }
    curLeft = curLeft + moveAmount;

    if (curLeft > maxX) {
        curLeft = maxX;
    } else if (curLeft < minX) {
        curLeft = minX
    }

    $container.css("left", curLeft + "px");
}