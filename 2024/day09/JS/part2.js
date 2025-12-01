const fs = require("fs");
const path = require("path");

const INPUT_FILE = path.join(__dirname, "../input.txt");

fs.readFile(INPUT_FILE, "utf-8", (err, input) => {
  if (err) {
    console.error(`Error reading file from path ${INPUT_FILE}:`, err);
    process.exit(1);
  }

  const data = input.split("").map(Number);

  const unpackedDisk = [];

  // Populate the unpackedDisk array
  for (let i = 0; i < data.length; i++) {
    const file = [];
    for (let j = data[i]; j > 0; j--) {
      if (i % 2 === 0) {
        file.push(i / 2);
      } else {
        file.push(".");
      }
    }
    if (file.length) {
      unpackedDisk.push(file);
    }
  }
  console.log(unpackedDisk);

  // Defragmentation process
  const movedIds = [];
  outer: for (let i = unpackedDisk.length - 1; i >= 0; i--) {
      if (unpackedDisk[i][0] !== '.' && !movedIds.includes(unpackedDisk[i][0])) {
          for (let j = 0; j <= i; j++) {
              if (
                  unpackedDisk[j][0] === '.' &&
                  unpackedDisk[j].length >= unpackedDisk[i].length
              ) {
                  if (unpackedDisk[j].length === unpackedDisk[i].length) {
                      movedIds.push(unpackedDisk[i][0]);
                      const temp = [...unpackedDisk[j]];
                      unpackedDisk[j] = unpackedDisk[i];
                      unpackedDisk[i] = temp;
                      continue outer;
                  }
                      movedIds.push(unpackedDisk[i][0]);
                      const temp = [...unpackedDisk[i]];
                      unpackedDisk[i].fill('.');
                      unpackedDisk.splice(
                          j,
                          1,
                          temp,
                          unpackedDisk[j].slice(unpackedDisk[i].length)
                      );
                      i++;
                      continue outer;
              }
          }
      }
  }
  const defraq= unpackedDisk.flat();
  let checkSum = 0;
  defraq.forEach((block, index) => {
    if (block !== ".") {
      checkSum += block * index;
    }
  });

  console.log(checkSum);
});