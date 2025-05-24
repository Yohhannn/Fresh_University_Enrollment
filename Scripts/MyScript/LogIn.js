$().ready(function () {
    $('#btn').click(function (e) {
        e.preventDefault(); // Prevent form submission if needed
        const student = {
            Stud_Code: $('#idNumber').val(),
            Stud_Password: $('#password').val()
        };

        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log(student);
        $.ajax({
            type: 'POST',
            url: '/Auth/LoginStudent',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: $.param(student) + "&__RequestVerificationToken=" + encodeURIComponent(token),
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Welcome!',
                        text: response.message,
                        confirmButtonText: 'Continue',
                    }).then(() => {
                        localStorage.setItem('studentData', JSON.stringify(response.data));
                        window.location.href = '/Main/Home';
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
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Unable to process login request.',
                });
            }
        });
    });
});