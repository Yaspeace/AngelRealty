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

function AddFileInput() {
    var div = document.getElementById("files");
    inp2 = document.createElement('input');
    inp2.type = 'file';
    inp2.name = 'Images';
    div.appendChild(inp2);
}

function AddFile() {
    var div_files = document.getElementById("files");
    div_files.insertAdjacentHTML('afterend', '<p><input type="file" name="Images" /></p>');
}

function OpenModal(adId) {
    document.getElementById('adId').value = adId;
    const modal = new bootstrap.Modal(document.querySelector('#modal'));
    modal.show();
}

function HideModal() {
    document.getElementById('adId').value = 0;
    const modal = new bootstrap.Modal(document.querySelector('#modal'));
    modal.hide();
}