const customName = document.getElementById('customname');
const randomize = document.querySelector('.randomize');
const story = document.querySelector('.story');

function randomValueFromArray(array){
  const random = Math.floor(Math.random()*array.length);
  return array[random];
}

let storyText = 'It was 94 fahrenheit outside, so :insertx: went for a walk. When they got to :inserty:, they stared in horror for a few moments, then :insertz:. Bob saw the whole thing, but was not surprised — :insertx: weighs 300 pounds, and it was a hot day.';

let insertX = ['Willy the Goblin','the soup kitchen','spontaneously combusted'];
let insertY = ['Big Daddy','Disneyland','melted into a puddle on the sidewalk'];
let insettZ = ['Father Christmas','the White House','turned into a slug and crawled away'];

randomize.addEventListener('click', result);

function result() {
  let newStory = storyText;
  let xItem = randomValueFromArray(insertX);
  let yItem = randomValueFromArray(insertY);
  let zItem = randomValueFromArray(insettZ);

  newStory = newStory.replace(/:insertx:/g,xItem);
  newStory = newStory.replace(":inserty:",yItem);
  newStory = newStory.replace(":insertz:",zItem);

  if(customName.value !== '') {
    let name = customName.value;
    newStory = newStory.replace('Bob',name);
  }

  if(document.getElementById("uk").checked) {
    let weight = Math.round(300* 0.0714286).toString() + ' stone';
    let temperature =  Math.round((94 - 32)/1.8).toString() + ' centigrade';
    newStory = newStory.replace('94 farenheit',weight);
    newStory = newStory.replace('300 pounds',temperature);
  }

  story.textContent = newStory;
  story.style.visibility = 'visible';
}
