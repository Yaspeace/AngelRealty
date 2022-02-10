// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function Liking(ad_id) {
    $.ajax({
        type: 'POST',
        data: { ad_id: ad_id },
        url: '/Home/Liking'
    }).done(function () {
        var lbl = document.getElementById("like" + String(ad_id));
        lbl.textContent == '♥' ? lbl.textContent = '♡' : lbl.textContent = '♥';
    });
}