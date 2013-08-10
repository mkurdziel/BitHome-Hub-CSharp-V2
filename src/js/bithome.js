function bithomeDisplayDate(ms) {
    var time = moment.unix(ms);

    return time.format("MM-DD-YYYY HH:mm:ss");
}