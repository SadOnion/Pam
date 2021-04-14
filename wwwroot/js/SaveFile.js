
function SaveFile(file) {
    var link = document.createElement('a');
    link.download = "Test.gif";
    link.href = file;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}