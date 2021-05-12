
function SaveFile(file,name) {
    var link = document.createElement('a');
    link.download = name;
    link.href = file;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}