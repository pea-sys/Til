const displayedImage = document.querySelector('.displayed-img');
const thumbBar = document.querySelector('.thumb-bar');

const btn = document.querySelector('button');
const overlay = document.querySelector('.overlay');

/* Looping through images */
function imageClick(e) 
{
    console.log(e.target.currentSrc);
    displayedImage.setAttribute('src',e.target.currentSrc);
}

for (let i = 1; i < 6; i++) {
    const newImage = document.createElement('img');
    newImage.setAttribute('src', "images/pic" + i.toString() + ".jpg");
    newImage.addEventListener('click',imageClick);
    thumbBar.appendChild(newImage); 
}


/* Wiring up the Darken/Lighten button */
function setBackColor(e)
{
  let btnCls = e.target.getAttribute("class");
    console.log(btnCls);
    if (btnCls === "dark")
    {
        e.target.setAttribute("class","light");
        e.target.textContent = "light";
        overlay.style.backgroundColor = "rgba(0,0,0,0.5)"; 
    }
    else
    {
        e.target.setAttribute("class","dark");
        e.target.textContent = "dark";
        overlay.style.backgroundColor = "rgba(0,0,0,0)"; 
    }
}
btn.addEventListener('click',setBackColor);