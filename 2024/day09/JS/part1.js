const fs = require("fs");
const path = require("path");

const INPUT_FILE = path.join(__dirname, "../input.txt");

fs.readFile(INPUT_FILE, "utf-8", (err, input) => {
  if (err) {
    console.error(`Error reading file from path ${INPUT_FILE}:`, err);
    process.exit(1);
  }

  const data = input.split("").map(Number);

  const unpeckedDisk = [];

  // Populate the unpeckedDisk array
  for (let i = 0; i < data.length; i++) {
    for (let j = data[i]; j > 0; j--) {
      if (i % 2 === 0) {
        unpeckedDisk.push(i / 2);
      } else {
        unpeckedDisk.push(".");
      }
    }
  }

  // Process the unpeckedDisk array
  unpeckedDisk.forEach((block, index) => {
    if (block === ".") {
      while (true) {
        if (unpeckedDisk.length === 0) {
          console.error("Error: unpeckedDisk array is empty while processing.");
          process.exit(1);
        }
        const temp = unpeckedDisk.pop();
        if (temp === ".") {
          continue;
        } else {
          unpeckedDisk[index] = temp;
          break;
        }
      }
    }
  });

  console.log(unpeckedDisk);

  // Calculate checkSum
  let checkSum = 0;
  unpeckedDisk.forEach((block, id) => {
    if (block !== ".") {
      checkSum += block * id;
    }
  });
  console.log(checkSum);
});