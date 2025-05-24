$(document).ready(function () {
    $('#btn').click(function (e) {
        e.preventDefault();

        let username = $('#username');
        let password = $('#password');

        // Trim values immediately
        let usernameVal = username.val().trim();
        let passwordVal = password.val().trim();

        let isValid = true;

        // Clear previous states
        username.removeClass('is-invalid');
        password.removeClass('is-invalid');

        // Username validation
        if (!usernameVal) {
            username.addClass('is-invalid');
            username.focus();
            isValid = false;
            $("#user-inv").removeAttr('hidden');
        }

        // Password validation (only check if username is valid)
        if (isValid && !passwordVal) {
            password.addClass('is-invalid');
            password.focus();
            isValid = false;
            $("#pass-inv").removeAttr('hidden');
        }

        if (!isValid) return;

        const faculty = {
            username: usernameVal, // Use trimmed value
            password: passwordVal  // Use trimmed value
        };

        console.log("Sending login request:", faculty); // Debug output

        $.ajax({
            type: 'POST',
            url: '/Auth/LoginFaculty',
            data: faculty,
            success: function (response) {
                console.log("Received response:", response); // Debug output
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Welcome!',
                        text: response.message,
                        confirmButtonText: 'Continue',
                    }).then(() => {
                        window.location.href = response.redirectUrl;
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Login Failed',
                        text: response.message,
                        confirmButtonText: 'Try Again'
                    });
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX error:", status, error); // Debug output
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Unable to process login request.',
                });
            }
        });
    });

    // Input event handlers
    $('#username, #password').on('input', function () {
        $(this).removeClass('is-invalid');
        $('#user-inv').attr('hidden', true);
        $('#pass-inv').attr('hidden', true);
    });
});