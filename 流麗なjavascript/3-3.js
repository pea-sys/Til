function countChar(str, char) {
    let count = 0;
    for (let i = 0; i < str.length; i++) {
        if (str[i] === char) {
            count++;
        }
    }
    return count;
}
function CountBs(str) {
    return countChar(str, "B");
}
console.log(CountBs("BBC"));
console.log(countChar("kakkerlak", "k"));