$(document).ready(function() {
    let allAcademicYears = [];

    // Step 1: Load all academic years on page load
    $.ajax({
        url: '/Curriculum/GetAllAcademicYears',  // Backend method to get all AYs
        type: 'GET',
        dataType: 'json'
    })
        .done(function(data) {
            allAcademicYears = data; // Store all academic years
            let options = '<option value="" selected disabled>Select Academic Year...</option>';
            data.forEach(function(ay) {
                options += `<option value="${ay.AyCode}">${ay.Display}</option>`;
            });
            $('#newAcademicYearSelect').html(options); // Do NOT disable here
        })
        .fail(function() {
            $('#newAcademicYearSelect')
                .html('<option selected disabled>Error loading Academic Years</option>')
                .prop('disabled', true);
        });

    // Step 2: When Program changes, filter AY dropdown to exclude assigned AYs
    $('#newProgramSelect').on('change', function() {
        const progCode = $(this).val();

        if (!progCode) {
            $('#newAcademicYearSelect')
                .html('<option value="" selected disabled>Select Academic Year...</option>')
                .prop('disabled', true);
            return;
        }

        $.ajax({
            url: '/Curriculum/GetAssignedAcademicYears',  // Backend method to get assigned AYs for the program
            type: 'GET',
            data: { progCode: progCode },
            dataType: 'json'
        })
            .done(function(assignedAys) {
                const assignedAyCodes = assignedAys.map(ay => ay.AyCode);
                const availableAys = allAcademicYears.filter(ay => !assignedAyCodes.includes(ay.AyCode));

                if (availableAys.length > 0) {
                    let options = '<option value="" selected disabled>Select Academic Year...</option>';
                    availableAys.forEach(function(ay) {
                        options += `<option value="${ay.AyCode}">${ay.Display}</option>`;
                    });
                    $('#newAcademicYearSelect').html(options).prop('disabled', false);
                } else {
                    $('#newAcademicYearSelect')
                        .html('<option selected disabled>No Academic Years available</option>')
                        .prop('disabled', true);
                }
            })
            .fail(function() {
                $('#newAcademicYearSelect')
                    .html('<option selected disabled>Error loading assigned Academic Years</option>')
                    .prop('disabled', true);
            });
    });

    // Step 3: Open Add Curriculum modal when button clicked (if applicable)
    $('#openAddCurriculumBtn').on('click', function() {
        $('#addCurriculumModal').modal('show');
    });

    // Step 4: Handle Add Curriculum form submission
    $('#addCurriculumForm').on('submit', function(e) {
        e.preventDefault();

        const progCode = $('#newProgramSelect').val();
        const ayCode = $('#newAcademicYearSelect').val();

        if (!progCode || !ayCode) {
            alert('Please select both Program and Academic Year.');
            return;
        }

        $.ajax({
            url: '/Curriculum/Add',
            type: 'POST',
            data: {
                ProgCode: progCode,
                AyCode: ayCode
            },
            dataType: 'json'
        })
            .done(function(response) {
                if (response.success) {
                    alert('Curriculum added successfully!');
                    $('#addCurriculumModal').modal('hide');
                    location.reload();
                } else {
                    alert('Error: ' + response.message);
                }
            })
            .fail(function() {
                alert('Error occurred while adding curriculum.');
            });
    });
});
