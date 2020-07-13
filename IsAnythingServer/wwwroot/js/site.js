$(function () {
    var typed = new Typed('#typed', {
        strings: ["Is 13 thirteen?", "Is youtube down?", "Is habr.com awesome?", "Is this pigeon?", "Is 2020 over?", "Is this joke?"],
        loop: true,
        backSpeed: 50,
        typeSpeed: 40
    });
});
$(function () {
    function tryParse(strict) {
        $("#read-input").removeClass(["is-invalid", "is-valid"]);
        $("#read-feedback").text("");
        let value = $("#read-input").val();
        let words = value.replace(/[? ]+$/g, '').trim().split(" ");
        if (words.length > 2) {
            if (words[words.length] === "?") {
                words.pop();
            }
            if (words[0].toLowerCase() !== "is") {
                if (strict) {
                    $("#read-input").addClass("is-invalid");
                }
                return null;
            } else {
                let splitIndex = words.map(word => word === "").lastIndexOf(true);
                splitIndex > 0 ? splitIndex : words.length - 1;
                let subject = words.slice(1, splitIndex).filter(e => e !== "");
                let predicate = words.slice(splitIndex, words.length).filter(e => e !== "");
                console.log(JSON.stringify({ subject: subject.join(" "), predicate: predicate.join(" ")}));
                $("#read-input").addClass("is-valid");
                $("#read-feedback").text(`Is {${subject.join(" ")}} {${predicate.join(" ")}}? Hint: use double space to separate subject and predicate.`);
                return { subject: subject.join(" "), predicate: predicate.join(" ") };
            }
        } else {
            if (strict) {
                $("#read-input").addClass("is-invalid");
            }
            return null;
        }
    }

    $("#read-input").on('input', function () {
        tryParse(false);
    });

    $("#read-form").submit(function (e) {
        function responseToText(response) {
            return "Is " + response.subject + " " + response.predicate + "? - " + valueToString(response.value);
        }

        function valueToString(value) {
            if (value === null) {
                return "Unknown";
            }
            return value ? "Yes" : "No";
        }

        let data = tryParse(true);
        if (data) {
            $("#read-submit").attr("disabled", true);
            $("#read-input").attr("disabled", true);
            $.ajax({
                url: "/api/is",
                type: "get", //send it through get method
                data: data,
                success: function (response) {
                    $("#read-result").text(responseToText(response));
                    $("#read-result").attr("hidden", false);
                    $("#override-link").attr("href", function (index, current) {
                        return `${current.split("?")[0]}?subject=${encodeURIComponent(response.subject)}&predicate=${encodeURIComponent(response.predicate)}`;
                    });
                    $("#override-text").attr("hidden", false);
                    $("#read-submit").attr("disabled", false);
                    $("#read-input").attr("disabled", false);
                },
                error: function (xhr) {
                    $("#read-result").text("Unknown error happened...");
                    $("#read-result").attr("hidden", false);
                    $("#override-text").attr("hidden", true);
                    $("#read-submit").attr("disabled", false);
                    $("#read-input").attr("disabled", false);
                }
            });
        } else {
            e.preventDefault();
        }
        e.preventDefault();
    });
});