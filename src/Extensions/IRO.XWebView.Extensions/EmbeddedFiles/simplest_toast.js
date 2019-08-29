(function () {
  if (window.IsSimplestToastInit)
    return;
  function addCss() {
    var style = document.createElement('style');
    style.id = 'simplestToastStyle';
    style.innerHTML = `
.toastdiv {
  visibility: hidden;
  min-width: 250px;
  margin-left: -125px;
  background-color: #333;
  color: #fff;
  text-align: center;
  border-radius: 2px;
  padding: 16px;
  position: fixed;
  z-index: 1;
  left: 50%;
  bottom: 30px;
  font-size: 17px;
  font-family:'Open Sans';

  visibility: visible;
  -webkit-animation: fadein 0.5s, fadeout 0.5s 2.5s;
  animation: fadein 0.5s, fadeout 0.5s 2.5s;
}


@-webkit-keyframes fadein {
  from {bottom: 0; opacity: 0;} 
  to {bottom: 30px; opacity: 1;}
}

@keyframes fadein {
  from {bottom: 0; opacity: 0;}
  to {bottom: 30px; opacity: 1;}
}

@-webkit-keyframes fadeout {
  from {bottom: 30px; opacity: 1;} 
  to {bottom: 0; opacity: 0;}
}

@keyframes fadeout {
  from {bottom: 30px; opacity: 1;}
  to {bottom: 0; opacity: 0;}
}
  `;
    document.head.appendChild(style);
  }
  addCss();
  window.SimplestToast = window.SimplestToast || {};
  window.SimplestToast.Show = function (msg, timeoutMS) {
    timeoutMS = isNaN(timeoutMS) ? 3000 : timeoutMS;
    var el = document.createElement('div');
    el.innerHTML = msg;
    el.className = "toastdiv";
    document.body.appendChild(el);
    setTimeout(function () { document.body.removeChild(el); }, timeoutMS);
  }
  window.IsSimplestToastInit = true;
})();