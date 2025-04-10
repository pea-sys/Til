function promise_all {
  return new Promise((resolve, reject) => {
    let result = [];
    let counter = 0;
    for (let i = 0; i < promises.length; i++) {
      promises[i].then(value => {
        result[i] = value;
        counter++;
        if (counter === promises.length) {
          resolve(result);
        }
      }).catch(reject);
    }
    if (promises.length === 0) {
      resolve(result);
    }
  }
}