const API_BASE_URL = "https://localhost:7226";

document.addEventListener("DOMContentLoaded", async () => {

    const adminMenuResponse = await fetch("/common/admin-menu.html");
    const adminMenuHtml     = await adminMenuResponse.text();

    document.getElementById("admin-menu").innerHTML = adminMenuHtml;

    const footerResponse = await fetch("/common/footer.html");
    const footerHtml     = await footerResponse.text();

    document.getElementById("footer").innerHTML = footerHtml;

    // 로그인 상태 확인
    try
    {
        const response = await fetch(`${API_BASE_URL}/api/Auth/me`,
            {
                credentials: "include"
            }
        );

        if (response.ok) {
            const user = await response.json();
            console.log("관리자 로그인 사용자:", user);
            console.log("권한:", user.role);

            // 관리자 권한 확인
            if (user.role !== "Admin") {
                alert("관리자 권한이 필요합니다.");

                location.href = "/main";

                return;
            }
            const adminUserId = document.getElementById("adminUserId");

            if (adminUserId)
            {
                adminUserId.textContent = `${user.userId} 접속 중`;
            }
        }
        else if (response.status === 401)
        {
            alert("로그인이 필요한 서비스입니다.");
            location.href = "/accounts/login";

            return;
        }
    }
    catch (error)
    {
        console.error("관리자 로그인 상태 확인 실패:", error);
    }
    // 로그아웃
    const logoutButton = document.getElementById("adminLogoutButton");

    if (logoutButton) {

        logoutButton.addEventListener(
            "click",
            async () => {
                await fetch(
                    `${API_BASE_URL}/api/Auth/logout`,
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