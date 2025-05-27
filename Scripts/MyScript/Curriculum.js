$(document).ready(function () {
    const $assignCourseModal = $('#assignCourseModal');
    const $modalProgramName = $('#modalProgramName');
    const $modalYearSemester = $('#modalYearSemester');
    const $availableCoursesList = $('#availableCoursesList');
    const $courseSearch = $('#courseSearch');
    const $confirmAssign = $('#confirmAssign');

    const $programSelect = $('#programSelect');
    const $yearLevelSelect = $('#yearLevelSelect');
    const $semesterSelect = $('#semesterSelect');
    const $academicYearSelect = $('#academicYearSelect');

    let selectedCoursesToAssign = new Set();
    let allAvailableCourses = [];

    // Dynamic academic years loading when program changes
    $programSelect.change(function () {
        var progCode = $(this).val();

        if (progCode) {
            $academicYearSelect.prop('disabled', true).html('<option>Loading...</option>');

            $.ajax({
                url: '/CurriculumCourse/GetAcademicYears',
                method: 'GET',
                data: { progCode: progCode },
                success: function (data) {
                    var options = '<option value="" disabled selected>Choose Academic Year...</option>';
                    if (data && data.length > 0) {
                        $.each(data, function (i, ay) {
                            options += '<option value="' + ay.AyCode + '">' + ay.DisplayText + '</option>';
                        });
                    } else {
                        options = '<option value="" disabled>No academic years found</option>';
                    }
                    $academicYearSelect.html(options);
                    $academicYearSelect.prop('disabled', false);
                },
                error: function () {
                    $academicYearSelect.html('<option value="" disabled>Error loading academic years</option>');
                    $academicYearSelect.prop('disabled', true);
                }
            });
        } else {
            $academicYearSelect.html('<option value="" disabled selected>Choose Academic Year...</option>');
            $academicYearSelect.prop('disabled', true);
        }
    });

    // Render courses with separator line between selected and unselected
    function renderAvailableCourses(selectedCourses, unselectedCourses) {
        $availableCoursesList.empty();

        if (selectedCourses.length === 0 && unselectedCourses.length === 0) {
            $availableCoursesList.append(
                '<tr><td colspan="8" class="text-center">No courses found.</td></tr>'
            );
            return;
        }

        function createCourseRow(course, isChecked) {
            return $(`
                <tr>
                    <td><input type="checkbox" class="course-checkbox" data-code="${course.code}" ${isChecked ? 'checked' : ''}></td>
                    <td>${course.code}</td>
                    <td>${course.title}</td>
                    <td>${course.category || ''}</td>
                    <td>${course.prerequisite || 'None'}</td>
                    <td>${course.units !== undefined ? course.units : ''}</td>
                    <td>${course.lec !== undefined ? course.lec : ''}</td>
                    <td>${course.lab !== undefined ? course.lab : ''}</td>
                </tr>
            `);
        }

        // Append selected courses first
        selectedCourses.forEach(course => {
            $availableCoursesList.append(createCourseRow(course, true));
        });

        // Add separator only if both selected and unselected courses exist
        if (selectedCourses.length > 0 && unselectedCourses.length > 0) {
            $availableCoursesList.append(`
                <tr><td colspan="8" style="border-top: 2px solid #007bff; padding: 0;"></td></tr>
            `);
        }

        // Append unselected courses next
        unselectedCourses.forEach(course => {
            $availableCoursesList.append(createCourseRow(course, false));
        });
    }

    // Fetch all courses and assigned courses, then render
    function fetchCourses() {
        const selectedProg = $programSelect.val();
        const selectedYear = $yearLevelSelect.val();
        const selectedSemester = $semesterSelect.val();
        const selectedAY = $academicYearSelect.val();

        $availableCoursesList.empty().append('<tr><td colspan="8" class="text-center">Loading courses...</td></tr>');

        return $.when(
            $.get('/Course/GetAllCourses', {
                progCode: selectedProg,
                ayCode: selectedAY
            }),
            $.get('/CurriculumCourse/GetAssignedCourses', {
                progCode: selectedProg,
                yearLevel: selectedYear,
                semester: selectedSemester,
                ayCode: selectedAY
            })
        ).done(function (allCoursesRes, assignedCoursesRes) {
            let allCourses = allCoursesRes[0];      // courses not assigned anywhere in this AY
            let assignedCourses = assignedCoursesRes[0];  // courses assigned in current prog + year + sem + AY

            // Convert codes to strings for consistent comparison
            const assignedCodesSet = new Set(assignedCourses.map(c => String(c.code)));

            // Add assigned courses for current year/sem to allCourses if missing
            assignedCourses.forEach(ac => {
                if (!allCourses.find(c => String(c.code) === String(ac.code))) {
                    allCourses.push(ac); // add missing assigned courses for current year/sem
                }
            });

            allAvailableCourses = allCourses;  // update global variable

            // Update selectedCoursesToAssign set with assigned courses for current year/semester
            selectedCoursesToAssign = new Set(assignedCodesSet);

            // Render with selected courses on top
            renderFilteredCourses();
    }).fail(function (jqXHR, textStatus, errorThrown) {
            console.error('❌ Failed to fetch courses: ', textStatus, errorThrown);
            console.log('🔍 Full response:', jqXHR.responseText);
            $availableCoursesList.empty().append(
                '<tr><td colspan="8" class="text-center text-danger">Failed to load courses.</td></tr>'
            );
        });
    }


    // Renders filtered & sorted courses based on current search and selected courses
    function renderFilteredCourses() {
        const query = $courseSearch.val().toLowerCase();

        // Filter courses by search term
        const filtered = allAvailableCourses.filter(c =>
            c.code.toLowerCase().includes(query) || c.title.toLowerCase().includes(query)
        );

        // Sort: selected courses first, then unselected
        const selectedCourses = filtered.filter(c => selectedCoursesToAssign.has(String(c.code)));
        const unselectedCourses = filtered.filter(c => !selectedCoursesToAssign.has(String(c.code)));

        renderAvailableCourses(selectedCourses, unselectedCourses);
    }

    // Show modal setup
    $assignCourseModal.on('show.bs.modal', function () {
        const selectedProg = $programSelect.val();
        const selectedYear = $yearLevelSelect.val();
        const selectedSemester = $semesterSelect.val();

        if (!selectedProg || !selectedYear || !selectedSemester) {
            alert('Please select Program, Year Level, and Semester before assigning courses.');
            return false;
        }

        $modalProgramName.text(selectedProg);
        $modalYearSemester.text(`${selectedYear} - ${selectedSemester}`);

        selectedCoursesToAssign.clear();
        $courseSearch.val('');
        fetchCourses();
    });

    // Course search filter
    $courseSearch.on('input', function () {
        renderFilteredCourses();
    });

    // Track selected courses and re-render on checkbox change
    $availableCoursesList.on('change', '.course-checkbox', function () {
        // Force courseCode as string here
        const courseCode = String($(this).data('code'));
        if ($(this).is(':checked')) {
            selectedCoursesToAssign.add(courseCode);
        } else {
            selectedCoursesToAssign.delete(courseCode);
        }
        // Re-render filtered list with checked courses on top
        renderFilteredCourses();
    });

    // Confirm assigning selected courses
    $confirmAssign.on('click', function () {
        selectedCoursesToAssign.clear();
        $availableCoursesList.find('input.course-checkbox:checked').each(function () {
            selectedCoursesToAssign.add(String($(this).data('code')));
        });

        if (selectedCoursesToAssign.size === 0) {
            alert('Please select at least one course to assign.');
            return;
        }

        const selectedProg = $programSelect.val();
        const selectedYear = $yearLevelSelect.val();
        const selectedSemester = $semesterSelect.val();
        const selectedAY = $academicYearSelect.val();

        function generateCurCode(progCode, academicYear, courseCode) {
            // Example: CURR-BSCS-2026-COURSECODE
            return `CURR-${progCode}-${academicYear.split('-')[0]}-${courseCode}`;
        }

        const dataToSend = Array.from(selectedCoursesToAssign).map(courseCode => ({
            curCode: generateCurCode(selectedProg, selectedAY, courseCode),
            crsCode: courseCode,
            curYearLevel: selectedYear,
            curSemester: selectedSemester,
            ayCode: selectedAY,
            progCode: selectedProg
        }));

        $.ajax({
            url: '/CurriculumCourse/AssignCourses',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataToSend),
            success: function (response) {
                if (response.success) {
                    alert('✅ Courses assigned successfully!');
                    $assignCourseModal.modal('hide');
                } else {
                    alert('❌ Error: ' + (response.message || 'Failed to assign courses.'));
                }
            },
            error: function () {
                alert('⚠️ An error occurred while assigning courses. Please try again.');
            }
        });
    });
});
