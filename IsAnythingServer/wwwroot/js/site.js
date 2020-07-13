$(function () {
    if ($("#typed").length) {
        var typed = new Typed('#typed', {
            strings: ["Is 13 thirteen?", "Is youtube down?", "Is habr.com awesome?", "Is this pigeon?", "Is 2020 over?", "Is this joke?"],
            loop: true,
            backSpeed: 50,
            typeSpeed: 40
        });
    }
});
$(function () {
    function tryParseSubjectAndPredicate(strict) {
        $("#subject-predicate-input").removeClass(["is-invalid", "is-valid"]);
        $("#subject-predicate-feedback").text("");
        let subjectAndPredicate = $("#subject-predicate-input").val();
        let words = subjectAndPredicate.replace(/[? ]+$/g, '').trim().split(" ");
        if (words.length > 2) {
            if (words[words.length] === "?") {
                words.pop();
            }
            if (words[0].toLowerCase() !== "is") {
                if (strict) {
                    $("#subject-predicate-input").addClass("is-invalid");
                }
                return null;
            } else {
                let splitIndex = words.map(word => word === "").lastIndexOf(true);
                splitIndex > 0 ? splitIndex : words.length - 1;
                let subject = words.slice(1, splitIndex).filter(e => e !== "");
                let predicate = words.slice(splitIndex, words.length).filter(e => e !== "");
                $("#subject-predicate-input").addClass("is-valid");
                $("#subject-predicate-feedback").text(`Is {${subject.join(" ")}} {${predicate.join(" ")}}? Hint: use double space to separate subject and predicate.`);
                return { subject: subject.join(" "), predicate: predicate.join(" ") };
            }
        } else {
            if (strict) {
                $("#subject-predicate-input").addClass("is-invalid");
            }
            return null;
        }
    }

    function tryParseValue(strict) {
        $("#value-input").removeClass(["is-invalid", "is-valid"]);
        let value = $("#value-input").val();
        if (value === "Yes" || value === "No") {
            $("#value-input").addClass("is-valid");
            return value === "Yes" ? true : false;
        } else {
            if (strict) {
                $("#value-input").addClass("is-invalid");
            }
            return null;
        }
    }

    function responseToText(response) {
        return "Is " + response.subject + " " + response.predicate + "? -> " + valueToString(response.value);
    }

    function valueToString(value) {
        if (value === null) {
            return "Unknown";
        }
        return value ? "Yes" : "No";
    }

    $("#subject-predicate-input").on('input', function () {
        tryParseSubjectAndPredicate(false);
    });

    $("#value-input").on('input', function () {
        tryParseValue(false);
    });

    $("#read-form").submit(function (e) {
        let data = tryParseSubjectAndPredicate(true);
        if (data) {
            $("#read-submit").attr("disabled", true);
            $("#subject-predicate-input").attr("disabled", true);
            $.ajax({
                url: "/api/is",
                type: "get",
                data: data,
                success: function (response) {
                    $("#read-result").text(responseToText(response));
                    $("#read-result").attr("hidden", false);
                    $("#result-link").attr("href", function (index, current) {
                        return `${current.split("?")[0]}?subject=${encodeURIComponent(response.subject)}&predicate=${encodeURIComponent(response.predicate)}`;
                    });
                    $("#result-text").attr("hidden", false);
                    $("#read-submit").attr("disabled", false);
                    $("#subject-predicate-input").attr("disabled", false);
                },
                error: function (xhr) {
                    $("#read-result").text("Unknown error happened...");
                    $("#read-result").attr("hidden", false);
                    $("#result-text").attr("hidden", true);
                    $("#read-submit").attr("disabled", false);
                    $("#subject-predicate-input").attr("disabled", false);
                }
            });
        } 
        e.preventDefault();
    });

    $("#write-form").submit(function (e) {
        let value = tryParseValue(true);

        let data = tryParseSubjectAndPredicate(true);
        if (data && value !== null) {
            data.value = value;
            $("#write-submit").attr("disabled", true);
            $("#value-input").attr("disabled", true);
            $("#subject-predicate-input").attr("disabled", true);
            $.ajax({
                url: "/api/is",
                type: "put",
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function (response) {
                    $("#write-result").text(responseToText(response));
                    $("#write-result").attr("hidden", false);
                    $("#write-submit").attr("disabled", false);
                    $("#value-input").attr("disabled", false);
                    $("#subject-predicate-input").attr("disabled", false);
                    $("#result-text").attr("hidden", false);
                },
                error: function (xhr) {
                    $("#write-result").text("Unknown error happened...");
                    $("#write-result").attr("hidden", false);
                    $("#write-submit").attr("disabled", false);
                    $("#value-input").attr("disabled", false);
                    $("#subject-predicate-input").attr("disabled", false);
                    $("#result-text").attr("hidden", true);
                }
            });
        }
        e.preventDefault();
    });
});