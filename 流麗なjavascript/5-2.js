function loop(value, test, update, body) {
  if (test(value)) {
    body(value);
    loop(update(value), test, update, body);
  }
}
loop(3, n => n > 0, n => n - 1, console.log);