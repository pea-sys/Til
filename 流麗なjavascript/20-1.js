const fs = require('fs');
const readline = require('readline');

async function grep(pattern, filePath) {
  try {
    const fileStream = fs.createReadStream(filePath);

    const rl = readline.createInterface({
      input: fileStream,
      crlfDelay: Infinity,
    });

    let lineNumber = 1;
    for await (const line of rl) {
      if (line.includes(pattern)) {
        console.log(`${filePath}:${lineNumber}:${line}`);
      }
      lineNumber++;
    }
  } catch (err) {
    console.error(`Error reading file: ${err.message}`);
  }
}

// コマンドライン引数の処理
const pattern = process.argv[2];
const filePath = process.argv[3];

if (!pattern || !filePath) {
  console.error('Usage: node grep.js <pattern> <file>');
  process.exit(1);
}

grep(pattern, filePath);