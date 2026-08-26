document.addEventListener("DOMContentLoaded", async () => {

    const menuResponse = await fetch("/common/menu.html");
    const menuHtml     = await menuResponse.text();

    document.getElementById("menu").innerHTML = menuHtml;

    const footerResponse = await fetch("/common/footer.html");
    const footerHtml     = await footerResponse.text();

    document.getElementById("footer").innerHTML = footerHtml;

// 로그인 링크 이벤트 등록
const loginLink = document.getElementById("loginLink");

    if (loginLink)
    {

    loginLink.addEventListener("click", e => {

        e.preventDefault();

        const currentUrl = location.pathname + location.search;

        location.href = `/accounts/login?returnUrl=${encodeURIComponent(currentUrl)}`;

        });
    }

    // 로그인 상태 확인
    try
    {
        const response = await fetch(
            "https://localhost:7226/api/Auth/me",
            {
                credentials: "include"
            }
        );
        if (response.ok)
        {
            const user = await response.json();

            console.log("로그인 사용자:", user);
            console.log("권한 :", user.role);

            document.getElementById("guestMenu").classList.add("d-none");
            document.getElementById("userMenu").classList.remove("d-none");

            let displayUserId = user.userId;

            if (displayUserId.length > 15)
            {
                displayUserId = displayUserId.substring(0, 15) + "...";
            }

            document.getElementById("menuUserName").textContent = displayUserId;
            console.log("로그인 사용자:", user);
        }
        else if (response.status === 401)
        {
            console.log("비로그인 상태");
        }
    }
    catch (error)
    {

        console.error("로그인 상태 확인 실패", error);
    }

    // 로그아웃
    const logoutButton = document.getElementById("logoutBtn");

    if (logoutButton)
    {
        logoutButton.addEventListener(
            "click",
            async () => {
                await fetch(
                    "https://localhost:7226/api/Auth/logout",
                    {
                        method: "POST",
                        credentials: "include"
                    }
                );
                location.href = "/main";
            }
        );
    }
});

