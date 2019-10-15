window.onload = function loadContributors() {
    fetch("https://api.github.com/repos/ChesterDevs/chester-devs-website/contributors")
        .then((response) => { if (response.ok) { return response.json() } })
        .then((contributors) => {
            let list = "";
            contributors.forEach((contributor, index) => {
                list += `<div style="clear:both; margin: 8px; height: 80px; width: 600px;"><img src="${contributor.avatar_url}" alt="${contributor.login}" style="float:left; width:80px;"><div style="float:left; margin-left: 12px;"><h3>#${index + 1} (<a href="https://github.com/ChesterDevs/chester-devs-website/commits/master?author=${contributor.login}">${contributor.contributions} commits</a>)</h3><h2>${contributor.login}</h2><a href="${contributor.url}">${contributor.url}</a></div></div>`
            })
            document.getElementById("contributer-list").innerHTML = list;
        })
};