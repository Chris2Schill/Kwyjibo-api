function populateStations(){
  var formStationName = document.getElementById('addStation_stationName');
  var form = document.getElementById("AddStationForm");

  for (i = 0; i < 10; i++){
    formStationName.value = 'Station'+i;
    form.submit();
  }
}

function removeOriginalIFrameBorders(){
  var frames = document.querySelectorAll("iframe");
  for (i = 0; i < frames.length; i++){
    frames[i].frameBorder = '0';
  }
}

removeOriginalIFrameBorders();

