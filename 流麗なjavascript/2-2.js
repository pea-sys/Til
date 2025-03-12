
for (let number = 1; number <=100; number++) {
  let s = "";
    if (number % 3 !== 0) {
        s = "Fizz";
    }
    else if (number % 5 !== 0) {
      s +="Buzz";
    }
    if (s !== "") {
    console.log(s);
    }
  } 