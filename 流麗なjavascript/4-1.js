function range(start, end) {
  return Array.from({ length: end - start + 1 }, (_, i) => start + i);
}
function sum(arr) {
  return arr.reduce((acc, cur) => acc + cur, 0);
}
console.log(sum(range(1, 10)));