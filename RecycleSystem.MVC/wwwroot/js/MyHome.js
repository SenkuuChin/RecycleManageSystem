layui.use(['element', 'jquery'], function () {


    var element = layui.element, $ = layui.$;

    var $body = $('body'),
        $tabBodys = $('#tab_Bodys');

    //模块方法集合
    var module = {

        //记录当前激活的选项卡索引
        activeTabIndex: 0,

        //根据激活的索引获取标签页内容
        tabBody: function (index) {
            return $tabBodys.find('.layadmin-tabsbody-item').eq(index || 0);
        },

        //打开标签页
        openTabPage: function (url, text) {
            var matchTo, //选项卡是否已经在已经打开的选项中匹配到
                $tabHeaders = $('#tab_Titles>li');//选项卡的标题集合

            //遍历选项卡，判断当前点击的菜单对应的标签是否存在
            $tabHeaders.each(function (index) {

                var $header = $(this),
                    layid = $header.attr('lay-id');

                if (layid === url) {
                    matchTo = true;
                    module.activeTabIndex = index;

                    //定位Tab
                    element.tabChange('layout-tab', url);
                }
            });

            //未在已打开的选项卡中匹配到，创建新的选项卡
            if (!matchTo) {

                //创建新的选项卡内容
                $tabBodys.append(' <div class="layadmin-tabsbody-item">\
                                <iframe src="'+ url + '" frameborder="0" class="layadmin-iframe" ></iframe >\
                                </div >');

                module.activeTabIndex = $tabHeaders.length;

                element.tabAdd('layout-tab',
                    {
                        title: text,
                        id: url,
                        attr: url
                    });

                //定位Tab
                element.tabChange('layout-tab', url);
            }


        },

        //切换选项卡
        tabChange: function (index, option) {
            option = option || {};

            var showClass = 'layui-show';

            module.tabBody(index).addClass(showClass).siblings().removeClass(showClass);

            module.resetMenu(option);
        },

        //重置菜单状态
        resetMenu: function (options) {
            var url = options.url,
                getMenuData = function (item) {
                    return {
                        navChildElem: item.children('.layui-nav-child'),
                        navLinkElem: item.children('*[lay-href]')
                    }
                },

                $sideMenu = $('#menu_Side'),
                itemedClass = 'layui-nav-itemed',
                thisClass = 'layui-this',

                //捕获当前激活的选项卡所对应的菜单
                matchMenu = function (list) {

                    list.each(function (firstIndex, firstItem) {

                        //判断当前选中的选项卡内容是否对应一级菜单
                        var firstElem = $(firstItem),
                            firstData = getMenuData(firstElem),
                            firstChildList = firstData.navChildElem.children('dd'),
                            isMatchFirst = url === firstData.navLinkElem.attr('lay-href');

                        //判断当前选中的选项卡内容是否对应一级菜单
                        firstChildList.each(function (secondIndex, secondItem) {
                            var secondElem = $(secondItem),
                                secondData = getMenuData(secondElem),
                                isMatchSecond = url === secondData.navLinkElem.attr('lay-href');

                            if (isMatchSecond) {
                                $(secondElem.parents('li')).addClass(itemedClass);
                                secondElem.addClass(thisClass);

                                return false;

                            }

                        });

                        if (isMatchFirst) {
                            firstElem.addClass(thisClass);

                            return false;
                        }

                    });
                }

            //清除所有的菜单选中状态
            $sideMenu.find('.' + itemedClass).removeClass(itemedClass);
            $sideMenu.find('.' + thisClass).removeClass(thisClass);

            //根据选中的选项卡状态设置菜单的选中状态
            matchMenu($sideMenu.children('li'));
        }

    }

    //模块内的事件集合
    var events = {

        //侧边导航栏伸缩事件
        flexible: function (that) {
            var sideShinkClass = 'layadmin-side-shrink',
                shrinkClass = 'layui-icon-shrink-right',
                spreadClass = 'layui-icon-spread-left';

            var $container = $('#cntr_Container'),
                $iconElem = $('#icon_Flexible'),
                isShrink = $container.hasClass(sideShinkClass);

            if (isShrink) {
                $container.removeClass(sideShinkClass); //展开导航栏
                $iconElem.removeClass(spreadClass).addClass(shrinkClass);
            } else {
                $container.addClass(sideShinkClass); //收缩导航栏
                $iconElem.addClass(spreadClass).removeClass(shrinkClass);
            }
        },

        //全屏展示
        fullScreen: function (that) {
            debugger;
            var screenFullClass = "layui-icon-screen-full",
                screenRestoreClass = 'layui-icon-screen-restore',
                iconElem = $(that).children('i');

            //非全屏状态下全屏展示
            if (iconElem.hasClass(screenFullClass)) {

                var bodyElem = document.body;

                if (bodyElem.webkitRequestFullScreen) {
                    bodyElem.webkitRequestFullScreen();
                } else if (bodyElem.mozRequestFullScreen) {
                    bodyElem.mozRequestFullScreen();
                } else {
                    bodyElem.requestFullScreen();
                }

                iconElem.addClass(screenRestoreClass).removeClass(screenFullClass);
            } else {
                var elem = document;

                if (elem.webkitCancelFullScreen) {
                    elem.webkitCancelFullScreen();
                } else if (elem.mozCancelFullScreen) {
                    elem.mozCancelFullScreen();
                } else {
                    elem.exitFullscreen();
                }

                iconElem.addClass(screenFullClass).removeClass(screenRestoreClass).addClass(screenFullClass);
            }

        },

        //关闭当前选项卡
        closeThis: function () {
            if (module.activeTabIndex !== 0) {
                $('#tab_Titles>li').eq(module.activeTabIndex).find('.layui-tab-close').trigger('click');
            }
        },

        //关闭其他标签页
        closeOther: function () {

            var removeClass = 'tab-remove';

            //将非当前标签页添加Class，再根据Class选择器删除
            $('#tab_Titles>li').each(function (index, item) {

                if (index > 0 && index !== module.activeTabIndex) {
                    $(item).addClass(removeClass);
                    module.tabBody(index).addClass(removeClass);
                }
            });

            $('.tab-remove').remove();

        },

        //关闭所有标签页
        closeAll: function () {

            //选中第一个标签页
            $('#tab_Titles>li').eq(0).trigger('click');

            var removeClass = 'tab-remove';

            //将非当前标签页添加Class，再根据Class选择器删除
            $('#tab_Titles>li').each(function (index, item) {

                if (index > 0 && index !== module.activeTabIndex) {
                    $(item).addClass(removeClass);
                    module.tabBody(index).addClass(removeClass);
                }
            });

            $('.tab-remove').remove();


        },

        //刷新选项卡
        refresh: function () {
            var iframClass = '.layadmin-iframe';
            var iframeElem = module.tabBody(module.activeTabIndex).find(iframClass);
            iframeElem[0].contentWindow.location.reload(true);
        }


    }

    //注册全局事件
    $body.on('click',
        '*[click-event]',
        function () {
            var that = $(this);

            //获取自定义的事件名称
            var eventAttr = that.attr('click-event');

            //执行事件
            events[eventAttr] && events[eventAttr](this);

        });

    //注册菜单导航点击事件
    $body.on('click',
        '*[lay-href]',
        function () {
            debugger;
            var href = $(this).attr('lay-href'),
                text = $(this).text();

            module.openTabPage(href, text);
        });

    //监听选项卡改变
    element.on('tab(layout-tab)',
        function (data) {
            var url = $(this).attr('lay-id'),
                index = data.index;

            module.activeTabIndex = index;

            module.tabChange(index,
                {
                    url: url
                });
        });

    //监听选项卡的删除
    element.on('tabDelete(layout-tab)',
        function (data) {

            var index = data.index;
            //module.activeTabIndex = index;

            module.tabBody(index).remove();


        });

})